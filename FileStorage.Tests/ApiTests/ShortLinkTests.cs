using BuisnessLogicLayer.Models;
using BuisnessLogicLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Controllers;

namespace FileStorage.Tests.ApiTests
{
    [Collection("Sequential")]
    public class ShortLinkTests
    {
        readonly string existingLink = TestHelper.ShortLinks[0].Link;
        readonly string wrongLink = TestHelper.ShortLinks[2].Link;
        readonly string notExistLink = "AAAAAA";

        [Fact]
        public async Task Can_DownLoad_File()
        {
            //Arrange
            var linkService = getShortLinkService();

            var controller = new ShortLinkController(linkService);

            //Act
            var result1 = ((await controller.DownloadFile(existingLink)) as FileContentResult);
            var result2 = ((await controller.DownloadFile(notExistLink)) as NotFoundResult);
            var result3 = ((await controller.DownloadFile(wrongLink)) as ForbidResult);

            //Assert
            Assert.NotNull(result1);
            Assert.Equal(TestHelper.fileContent.Length, result1?.FileContents?.Length);
            Assert.Equal(TestHelper.FileDatas[0].UntrustedName, result1?.FileDownloadName);
            Assert.NotNull(result2);
            Assert.Equal(StatusCodes.Status404NotFound, result2?.StatusCode);
            Assert.NotNull(result3);
        }

        [Fact]
        public async Task Can_Get_Preview()
        {
            //Arrange
            var linkService = getShortLinkService();

            var controller = new ShortLinkController(linkService);

            //Act
            var result1 = ((await controller.GetFileData(existingLink)) as OkObjectResult);
            var result2 = ((await controller.GetFileData(notExistLink)) as NotFoundResult);
            var result3 = ((await controller.GetFileData(wrongLink)) as ForbidResult);

            //Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
        }

        [Fact]
        public async Task Can_Create_Short_Link()
        {
            //Arrange
            var mapper = TestHelper.CreateMapperProfile();
            var linkService = getShortLinkService();
            var controller = new ShortLinkController(linkService);

            var fdmClosed = mapper.Map<FileDataModel>(TestHelper.FileDatas[1]);
            var fdmWithLink = mapper.Map<FileDataModel>(TestHelper.FileDatas[0]);
            var fdnNF = mapper.Map<FileDataModel>(TestHelper.FileDatas[0]);
            fdnNF.Id = Guid.NewGuid();
            var fdm = mapper.Map<FileDataModel>(TestHelper.FileDatas[2]);

            //Act
            var ok = (await controller.CreateShortlink(fdm.Id, fdm)) as OkObjectResult;
            var badReqk = (await controller.CreateShortlink(Guid.NewGuid(), fdm)) as BadRequestResult;
            var forbid = (await controller.CreateShortlink(fdmClosed.Id, fdmClosed)) as ForbidResult;
            var notFound = (await controller.CreateShortlink(fdnNF.Id, fdnNF)) as NotFoundResult;
            var linkExist = (await controller.CreateShortlink(fdmWithLink.Id, fdmWithLink)) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(ok);
            Assert.NotNull(badReqk);
            Assert.NotNull(forbid);
            Assert.NotNull(notFound);
            Assert.NotNull(linkExist);
        }

        [Fact]
        public async Task Can_Delete_Link()
        {
            //Arrange
            var mapper = TestHelper.CreateMapperProfile();
            var linkService = getShortLinkService();
            var controller = new ShortLinkController(linkService);

            var fdm = mapper.Map<FileDataModel>(TestHelper.FileDatas[0]);
            var fdmNoLink = mapper.Map<FileDataModel>(TestHelper.FileDatas[1]);
            fdmNoLink.ShortLink = notExistLink;

            //Act
            var ok = (await controller.DeleteShortLink(existingLink, fdm)) as OkObjectResult;
            var badReqk = (await controller.DeleteShortLink(wrongLink, fdm)) as BadRequestResult;
            var notFound = (await controller.DeleteShortLink(notExistLink, fdmNoLink)) as NotFoundResult;
            

            //Assert
            Assert.NotNull(ok);
            Assert.NotNull(badReqk);
            Assert.NotNull(notFound);
        }

        private ShortLinkService getShortLinkService()
        {
            var mockLogger = new Mock<ILogger<UnitOfWork>>();
            var mockUserManager = TestHelper.MockUserManager<AppUser>();
            var mockRoleManager = TestHelper.MockRoleManager<IdentityRole<Guid>>();
            var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());

            var unitOfWork = new UnitOfWork(context, mockLogger.Object,
                mockUserManager.Object, mockRoleManager.Object);
            var mapper = TestHelper.CreateMapperProfile();

            return new ShortLinkService(unitOfWork, mapper);
        }
    }
}
