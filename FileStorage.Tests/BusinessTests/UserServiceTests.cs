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
    public class UserServiceTests
    {
        [Fact]
        public async Task Check_Is_User_Exist()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var userService = new UserService(unitOfWork, mapper);

            //Act
            var result1 = await userService.IsExistByEmailAsync(TestHelper.Users[0].Email);
            var result2 = await userService.IsExistByEmailAsync("noExist@mail.com");
            var result3 = await userService.IsExistByEmailAsync("notEmail");

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.Error, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Get_UserByEmail()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var userService = new UserService(unitOfWork, mapper);

            //Act
            var result1 = await userService.GetByEmailAsync(TestHelper.Users[0].Email);
            var result2 = await userService.GetByEmailAsync("noExist@mail.com");
            var result3 = await userService.GetByEmailAsync("notEmail");

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.Users[0].UserName, result1?.Data?.Name);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.Error, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Get_All_Users()
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

            var userService = new UserService(unitOfWork, mapper);

            //Act
            var result1 = await userService.GetAllAsync(query);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.Users.Count, result1?.Data?.TotalCount);
        }

        [Fact]
        public async Task Can_Get_By_Id()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var userService = new UserService(unitOfWork, mapper);

            //Act
            var result1 = await userService.GetByIdAsync(TestHelper.Users[0].Id);
            var result2 = await userService.GetByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.Users[0].UserName, result1?.Data?.Name);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
        }

        [Fact]
        public async Task Can_Delete_User()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            mockUserManager.Setup(um => um.IsInRoleAsync(It.Is<AppUser>(au => au.UserName == "User3"), 
                It.Is<string>(s => s == "Administrator"))).Returns(Task.Run(() => true));

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var user1 = mapper.Map<UserModel>(TestHelper.Users[0]);
            var user2 = mapper.Map<UserModel>(TestHelper.Users[2]);
            var query = new QueryModel();

            var userService = new UserService(unitOfWork, mapper);

            //Act
            var result1 = await userService.DeleteAsync(user1);
            var result2 = await userService.DeleteAsync(user2);
            var count = (await userService.GetAllAsync(query))?.Data?.TotalCount;

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result2.ResponseResult);
            Assert.Equal(3, count);
            mockUserManager.Verify(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.Is<string>(s => s == "Administrator")), Times.Exactly(2));
        }

        [Fact]
        public async Task Can_Update_User()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            mockUserManager.Setup(um => um.FindByIdAsync(It.Is<string>(s => s == TestHelper.Users[0].Id.ToString())))
                .Returns(Task.Run(() => TestHelper.Users[0]));
            mockUserManager.Setup(um => um.FindByIdAsync(It.Is<string>(s => s == TestHelper.Users[3].Id.ToString())))
                .Returns(Task.Run(() => TestHelper.Users[3]));

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var user1 = mapper.Map<UserModel>(TestHelper.Users[0]);
            var user2 = mapper.Map<UserModel>(TestHelper.Users[3]);

            var userService = new UserService(unitOfWork, mapper);

            //Act
            var result1 = await userService.UpdateAsync(user1);
            var result2 = await userService.UpdateAsync(user2);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(ResponseResult.Success, result2.ResponseResult);
            mockUserManager.Verify(um => um.FindByIdAsync(It.Is<string>(s => s == TestHelper.Users[0].Id.ToString())), Times.Once);
            mockUserManager.Verify(um => um.FindByIdAsync(It.Is<string>(s => s == TestHelper.Users[3].Id.ToString())), Times.Once);
            mockUserManager.Verify(um => um.UpdateAsync(It.IsAny<AppUser>()), Times.Exactly(2));
        }
    }
}
