using DataAccessLayer.Data;
using DataAccessLayer.Repositories;

namespace FileStorage.Tests.DalTests
{
    [Collection("Sequential")]
    public class ShortLinkRepositoryTests
    {
        [Fact]
        public async Task Can_Find_Collisions()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var linkRepo = new ShortLinkRepository(context);
            var existingLink = TestHelper.ShortLinks[0].Link;
            var notExistYet = "AAAAAA";

            //Act
            var check1 = await linkRepo.IsCollision(existingLink);
            var check2 = await linkRepo.IsCollision(notExistYet);

            //Assert
            Assert.True(check1);
            Assert.False(check2);
        }

        [Fact]
        public async Task Can_Find_Existed()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var linkRepo = new ShortLinkRepository(context);

            //Act
            var check1 = await linkRepo.IsExist(TestHelper.FileDatas[0].Id);
            var check2 = await linkRepo.IsExist(TestHelper.FileDatas[4].Id);

            //Assert
            Assert.True(check1);
            Assert.False(check2);
        }

        [Fact]
        public async Task Can_Get_FileContent_ByLink()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var linkRepo = new ShortLinkRepository(context);
            var existingLink = TestHelper.ShortLinks[0].Link;

            //Act
            var content = (await linkRepo.GetFileContentByLinkAsync(existingLink))?.AppFileNav?.Content;

            //Assert
            Assert.Equal(TestHelper.fileContent, content);
        }

        [Fact]
        public async Task Can_Get_FileData_ByLink()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var linkRepo = new ShortLinkRepository(context);
            var existingLink = TestHelper.ShortLinks[0].Link;

            //Act
            var file = await linkRepo.GetFileDataByLinkAsync(existingLink);

            //Assert
            Assert.Equal(TestHelper.FileDatas[0].Id, file?.Id);
            Assert.Equal(TestHelper.FileDatas[0].UntrustedName, file?.UntrustedName);
        }

        [Fact]
        public async Task Can_Get_ShortLink_Object_By_Link()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var linkRepo = new ShortLinkRepository(context);
            var existingLink = TestHelper.ShortLinks[0].Link;

            //Act
            var link = await linkRepo.GetShortLinkAsync(existingLink);

            //Assert
            Assert.Equal(TestHelper.FileDatas[0].Id, link?.AppFileDataId);
        }

        [Fact]
        public async Task Can_Get_ShortLink_ObjectWitnRelated_By_Link()
        {
            //Arrange
            using var context = new AppDbContext(TestHelper.GetUnitTestDbOptions());
            var linkRepo = new ShortLinkRepository(context);
            var existingLink = TestHelper.ShortLinks[0].Link;

            //Act
            var link = await linkRepo.GetShortLinkWithRelatedAsync(existingLink);

            //Assert
            Assert.Equal(TestHelper.FileDatas[0].Id, link?.AppFileDataId);
            Assert.Equal(TestHelper.FileDatas[0].OwnerId, link?.AppFileDataNav?.OwnerId);
            Assert.Equal(TestHelper.FileDatas[0].UploadDT, link?.AppFileDataNav?.UploadDT);
        }
    }
}
