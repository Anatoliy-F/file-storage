using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Models;
using BuisnessLogicLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace FileStorage.Tests.BusinessTests
{
    [Collection("Sequential")]
    public class FileServiceTests
    {
        [Fact]
        public async Task Can_Get_Own_ById()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetOwnByIdAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[0].Id);
            var result2 = await fileService.GetOwnByIdAsync(TestHelper.Users[0].Id, Guid.NewGuid());
            var result3 = await fileService.GetOwnByIdAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[4].Id);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.FileDatas[0].UntrustedName, result1?.Data?.Name);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Add_New_File()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result = await fileService.AddFromScratch(
                "newFile.txt",
                "Add during test",
                TestHelper.Users[0].Id,
                TestHelper.fileContent
                );
            var count = await unitOfWork.AppFileDataRepository.GetFilesCountAsync();

            //Assert
            Assert.Equal(ResponseResult.Success, result.ResponseResult);
            Assert.Equal(TestHelper.fileContent.LongLength, result?.Data?.Size);
            Assert.Equal("newFile.txt", result?.Data?.Name);
            Assert.Equal("Add during test", result?.Data?.Note);
            Assert.Equal(9, count);
        }

        [Fact]
        public async Task Can_Get_Shared_ById()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetSharedByIdAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[6].Id);
            var result2 = await fileService.GetSharedByIdAsync(TestHelper.Users[0].Id, Guid.NewGuid());
            var result3 = await fileService.GetSharedByIdAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[3].Id);
            var result4 = await fileService.GetSharedByIdAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[4].Id);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.FileDatas[6].UntrustedName, result1?.Data?.Name);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result4.ResponseResult);
        }

        [Fact]
        public async Task Can_Get_ById()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetByIdAsync(TestHelper.FileDatas[0].Id);
            var result2 = await fileService.GetByIdAsync(Guid.NewGuid());
            

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.FileDatas[0].UntrustedName, result1?.Data?.Name);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
        }

        [Fact]
        public async Task Can_Update_Own()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileData1 = mapper.Map<FileDataModel>(TestHelper.FileDatas[0]);
            fileData1.Name = "new Name";
            fileData1.Note = "new Note";
            var fileData2 = mapper.Map<FileDataModel>(TestHelper.FileDatas[1]);
            fileData2.Id = Guid.NewGuid();
            var fileData3 = mapper.Map<FileDataModel>(TestHelper.FileDatas[2]);
            var fileData4 = mapper.Map<FileDataModel>(TestHelper.FileDatas[1]);
            fileData4.ShortLink = "afsdad";

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.UpdateOwnAsync(TestHelper.Users[0].Id, fileData1);
            var result2 = await fileService.UpdateOwnAsync(TestHelper.Users[0].Id, fileData2);
            var result3 = await fileService.UpdateOwnAsync(TestHelper.Users[0].Id, fileData3);
            var result4 = await fileService.UpdateOwnAsync(TestHelper.Users[0].Id, fileData4);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
            Assert.Equal(ResponseResult.Error, result4.ResponseResult);
        }

        [Fact]
        public async Task Can_Update()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileData1 = mapper.Map<FileDataModel>(TestHelper.FileDatas[0]);
            fileData1.Name = "new Name";
            fileData1.Note = "new Note";
            var fileData2 = mapper.Map<FileDataModel>(TestHelper.FileDatas[1]);
            fileData2.Id = Guid.NewGuid();
            var fileData3 = mapper.Map<FileDataModel>(TestHelper.FileDatas[1]);
            fileData3.ShortLink = "afsdad";

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.UpdateAsync(fileData1);
            var result2 = await fileService.UpdateAsync(fileData2);
            var result3 = await fileService.UpdateAsync(fileData3);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.Error, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Get_Own_Content()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetOwnContentAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[0].Id);
            var result2 = await fileService.GetOwnContentAsync(TestHelper.Users[0].Id, Guid.NewGuid());
            var result3 = await fileService.GetOwnContentAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[4].Id);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.FileDatas[0].UntrustedName, result1?.Data?.Name);
            Assert.Equal(TestHelper.fileContent, result1?.Data?.Content);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Get_Content()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetContentAsync(TestHelper.FileDatas[0].Id);
            var result2 = await fileService.GetContentAsync(Guid.NewGuid());


            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.FileDatas[0].UntrustedName, result1?.Data?.Name);
            Assert.Equal(TestHelper.fileContent, result1?.Data?.Content);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
        }

        [Fact]
        public async Task Can_Get_All_Own()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var query = new QueryModel();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetAllOwnAsync(TestHelper.Users[0].Id, query);
            var result2 = await fileService.GetAllOwnAsync(Guid.NewGuid(), query);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(2, result1?.Data?.TotalCount);
            Assert.Equal(ResponseResult.Success, result2.ResponseResult);
            Assert.Equal(0, result2?.Data?.TotalCount);
        }

        [Fact]
        public async Task Can_Get_All()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var query = new QueryModel();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetAllAsync(query);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(8, result1?.Data?.TotalCount);
        }

        [Fact]
        public async Task Can_Get_Shared()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var query = new QueryModel();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.GetSharedAsync(TestHelper.Users[0].Id, query);
            var result2 = await fileService.GetSharedAsync(Guid.NewGuid(), query);
            var result3 = await fileService.GetSharedAsync(TestHelper.Users[3].Id, query);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(2, result1?.Data?.TotalCount);
            Assert.Equal(ResponseResult.Error, result2.ResponseResult);
            Assert.Equal(ResponseResult.Success, result3.ResponseResult);
            Assert.Equal(0, result3?.Data?.TotalCount);
        }

        [Fact]
        public async Task Can_Delete()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var resp = unitOfWork.AppFileDataRepository.GetAllNoTraking().First(e => e.Id == TestHelper.FileDatas[5].Id);
            var file = mapper.Map<FileDataModel>(resp);

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result = await fileService.DeleteAsync(file);
            var count = await unitOfWork.AppFileDataRepository.GetFilesCountAsync();

            //Assert
            Assert.Equal(ResponseResult.Success, result.ResponseResult);
            Assert.Equal(7, count);
        }

        [Fact]
        public async Task Can_Delete_Own()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.DeleteOwnAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[0].Id);
            var result2 = await fileService.DeleteOwnAsync(TestHelper.Users[0].Id, Guid.NewGuid());
            var result3 = await fileService.DeleteOwnAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[4].Id);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Refuse_Shared()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.RefuseSharedAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[6].Id);
            var result2 = await fileService.RefuseSharedAsync(TestHelper.Users[0].Id, Guid.NewGuid());
            var result3 = await fileService.RefuseSharedAsync(TestHelper.Users[0].Id, TestHelper.FileDatas[4].Id);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Share_By_Email()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var fileService = new FileService(unitOfWork, mapper);

            //Act
            var result1 = await fileService.ShareByEmailAsync(TestHelper.Users[0].Id, "user3@mail.com", TestHelper.FileDatas[1].Id);
            var result2 = await fileService.ShareByEmailAsync(TestHelper.Users[0].Id, "user13@mail.com", TestHelper.FileDatas[1].Id);
            var result3 = await fileService.ShareByEmailAsync(TestHelper.Users[0].Id, "user3@mail.com", Guid.NewGuid());
            var result4 = await fileService.ShareByEmailAsync(TestHelper.Users[0].Id, "user3@mail.com", TestHelper.FileDatas[3].Id);
            var result5 = await fileService.ShareByEmailAsync(TestHelper.Users[0].Id, "wrongMail", TestHelper.FileDatas[0].Id);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result3.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result4.ResponseResult);
            Assert.Equal(ResponseResult.Error, result5.ResponseResult);
        }
    }
}
