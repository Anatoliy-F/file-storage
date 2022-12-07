using DataAccessLayer.Data;
using DataAccessLayer.Repositories;

namespace FileStorage.Tests.DalTests
{
    [Collection("Sequential")]
    public class AppUserRepositoryTests
    {
        [Fact]
        public async Task Can_Get_By_Id_With_Related()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var userRepo = new AppUserRepository(context);

            //Act
            var user = await userRepo.GetByIdWithRelatedAsync(TestHelper.Users[0].Id);

            //Assert
            Assert.NotNull(user);
            Assert.Equal(2, user!.AppFiles.Count);
            Assert.Equal(2, user!.ReadOnlyFiles.Count);
        }

        [Fact]
        public async Task Can_Count_ReadOnly_Files()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var userRepo = new AppUserRepository(context);

            //Act
            var count = await userRepo.GetReadOnlyFilesCountAsync(TestHelper.Users[0].Id);

            //Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Can_Count_Users()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var userRepo = new AppUserRepository(context);

            //Act
            var count = await userRepo.GetUsersCountAsync();

            //Assert
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task Can_Retrieve_User_By_Email()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var userRepo = new AppUserRepository(context);

            //Act
            var user = await userRepo.GetByEmailAsync(TestHelper.Users[0].Email);

            //Assert
            Assert.Equal(TestHelper.Users[0].Id, user?.Id);
            Assert.Equal(TestHelper.Users[0].UserName, user?.UserName);
        }

        [Fact]
        public async Task Check_User_By_Email()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var userRepo = new AppUserRepository(context);

            //Act
            var isExist1 = await userRepo.IsExistByEmailAsync(TestHelper.Users[0].Email);
            var isExist2 = await userRepo.IsExistByEmailAsync("wrong@mail.com");

            //Assert
            Assert.True(isExist1);
            Assert.False(isExist2);
        }

        [Fact]
        public async Task Can_Get_All_No_Tracking()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var userRepo = new AppUserRepository(context);

            //Act
            var allUsers = userRepo.GetAllNoTracking().ToList();
            allUsers[0].UserName = "AAA";
            var id = allUsers[0].Id;
            context.SaveChanges();
            var user = await userRepo.GetByIdAsync(id);

            //Assert
            Assert.NotEqual("AAA", user?.UserName);
        }
    }
}
