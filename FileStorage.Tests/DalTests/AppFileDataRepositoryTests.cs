using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Tests.DalTests
{
    public class AppFileDataRepositoryTests
    {
        [Fact]
        public void Can_Get_All()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var fileRepo = new AppFileDataRepository(context);

            //Act
            var files = fileRepo.GetAll().ToList();
            
            //Assert
            Assert.Equal(TestHelper.FileDatas.Count, files.Count);
        }

        [Fact]
        public async Task Can_Get_By_Id_With_Content()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            var file = await fileRepo.GetByIdWithContentAsync(TestHelper.FileDatas[0].Id);

            //Assert
            Assert.Equal(TestHelper.fileContent, file?.AppFileNav?.Content);
        }

        [Fact]
        public async Task Can_Get_By_Id_With_Related()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            var file = await fileRepo.GetByIdWithRelatedAsync(TestHelper.FileDatas[0].Id);

            //Assert
            Assert.Equal(
                TestHelper.Users.FirstOrDefault(u => u.Id == file!.OwnerId)?.UserName, 
                file?.OwnerNav?.UserName);
        }

        [Fact]
        public async Task Can_Count_User_Files()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            int count = await fileRepo.GetUserFilesCountAsync(TestHelper.Users[0].Id);

            //Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void Can_Get_Shared()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            var shared = fileRepo.GetShared(TestHelper.Users[0].Id).AsEnumerable().ToList();

            //Assert
            Assert.Equal(2, shared.Count);
            Assert.Equal(TestHelper.Users[3].Id, shared[0].OwnerId);
            Assert.Equal(TestHelper.Users[3].Id, shared[1].OwnerId);
        }

        [Fact]
        public async Task Can_Count_Files()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            int count = await fileRepo.GetFilesCountAsync();

            //Assert
            Assert.Equal(8, count);
        }

        [Fact]
        public async Task Can_Get_All_No_Tracking()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            var allFiles = fileRepo.GetAllNoTraking().ToList();
            allFiles[0].UntrustedName = "AAA";
            var id = allFiles[0].Id;
            context.SaveChanges();
            var file = await fileRepo.GetByIdAsync(id);

            //Assert
            Assert.NotEqual("AAA", file?.UntrustedName);   
        }

        [Fact]
        public async Task Can_Check_Owner()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            var isOwner1 = await fileRepo.IsOwner(TestHelper.FileDatas[0].Id, TestHelper.Users[0].Id);
            var isOwner2 = await fileRepo.IsOwner(TestHelper.FileDatas[0].Id, TestHelper.Users[1].Id);

            //Assert
            Assert.True(isOwner1);
            Assert.False(isOwner2);
        }

        [Fact]
        public async Task Can_Find_By_Id()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            var file0 = await fileRepo.GetByIdAsync(TestHelper.FileDatas[0].Id);
            var file4 = await fileRepo.GetByIdAsync(TestHelper.FileDatas[4].Id);
            var file9 = await fileRepo.GetByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Equal(TestHelper.FileDatas[0].OwnerId, file0?.OwnerId);
            Assert.Equal(TestHelper.FileDatas[4].OwnerId, file4?.OwnerId);
            Assert.Equal(TestHelper.FileDatas[0].UntrustedName, file0?.UntrustedName);
            Assert.Equal(TestHelper.FileDatas[4].UntrustedName, file4?.UntrustedName);
            Assert.Null(file9);
        }

        [Fact]
        public async Task Can_Add_And_Delete_Async()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);
            var newFileData = new AppFileData
            {
                Id = Guid.NewGuid(),
                UntrustedName = "Test.txt",
                Size = 11,
                UploadDT = DateTime.Parse("11/11/2022 14:00:00"),
                OwnerId = TestHelper.Users[2].Id,
                IsPublic = false
            };

            //Act
            int countBeforeAdd = context.AppFilesData.Count();
            await fileRepo.AddAsync(newFileData);
            await context.SaveChangesAsync();
            int countAfterAdd = context.AppFilesData.Count();

            fileRepo.Delete(newFileData);
            await context.SaveChangesAsync();
            int countAfterDelete = context.AppFilesData.Count();

            //Assert
            Assert.Equal(countBeforeAdd, countAfterDelete);
            Assert.Equal(countBeforeAdd + 1, countAfterAdd);
        }

        [Fact]
        public async Task Can_Update_Async()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var fileRepo = new AppFileDataRepository(context);

            //Act
            var fileForUpdate = await fileRepo.GetByIdAsync(TestHelper.FileDatas[7].Id);
            var newName = fileForUpdate?.UntrustedName + "TestUpdate";
            fileForUpdate!.UntrustedName = newName;
            fileRepo.Update(fileForUpdate);
            await context.SaveChangesAsync();
            var fileAfterUpdate = await fileRepo.GetByIdAsync(TestHelper.FileDatas[7].Id);

            //Assert
            Assert.Equal(newName, fileAfterUpdate?.UntrustedName);
        }
    }
}
