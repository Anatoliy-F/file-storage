using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Tests.BusinessTests
{
    [Collection("Sequential")]
    public class ShortLinkServiceTests
    {
        readonly string existingLink = TestHelper.ShortLinks[0].Link;
        readonly string wrongLink = TestHelper.ShortLinks[2].Link;
        readonly string notExistLink = "AAAAAA";

        [Fact]
        public async Task Can_Get_File_ByShortLink()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object, 
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var linkService = new ShortLinkService(unitOfWork, mapper);

            //Act
            var result1 = await linkService.GetFileByShortLinkAsync(existingLink);
            var result2 = await linkService.GetFileByShortLinkAsync(notExistLink);
            var result3 = await linkService.GetFileByShortLinkAsync(wrongLink);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.fileContent, result1.Data?.Content);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Get_FileData_ByShortLink()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.TestUserManager<AppUser>(null!);
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var linkService = new ShortLinkService(unitOfWork, mapper);

            //Act
            var result1 = await linkService.GetShortFileDataAsync(existingLink);
            var result2 = await linkService.GetShortFileDataAsync(notExistLink);
            var result3 = await linkService.GetShortFileDataAsync(wrongLink);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.FileDatas[0].Note, result1.Data?.Note);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
        }

        [Fact]
        public async Task Can_Generate_ShortLink()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.TestUserManager<AppUser>(null!);
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var linkService = new ShortLinkService(unitOfWork, mapper);

            //Act
            var result1 = await linkService.GenerateForFileByIdAsync(TestHelper.FileDatas[4].Id);
            var result2 = await linkService.GenerateForFileByIdAsync(Guid.NewGuid());
            var result3 = await linkService.GenerateForFileByIdAsync(TestHelper.FileDatas[5].Id);
            var result4 = await linkService.GenerateForFileByIdAsync(TestHelper.FileDatas[7].Id);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult);
            Assert.Equal(TestHelper.FileDatas[4].Id, result1?.Data?.FileId);
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.AccessDenied, result3.ResponseResult);
            Assert.Equal(ResponseResult.Error, result4.ResponseResult);
        }

        [Fact]
        public async Task Can_Delete_ShortLink()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.TestUserManager<AppUser>(null!);
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            var linkService = new ShortLinkService(unitOfWork, mapper);

            //Act
            var result1 = await linkService.DeleteLinkAsync(existingLink);
            var result2 = await linkService.DeleteLinkAsync(notExistLink);

            var result3 = await linkService.GetShortFileDataAsync(existingLink);

            //Assert
            Assert.Equal(ResponseResult.Success, result1.ResponseResult); 
            Assert.Equal(ResponseResult.NotFound, result2.ResponseResult);
            Assert.Equal(ResponseResult.NotFound, result3.ResponseResult);
        }
    }
}
