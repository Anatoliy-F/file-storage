using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace FileStorage.Tests
{
    internal static class TestHelper
    {
        public static DbContextOptions<AppDbContext> GetUnitTestDbOptions()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new AppDbContext(options))
            {
                SeedData(context);
            }

            return options;
        }
            
        public static void SeedData(AppDbContext context)
        {
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            context.AppUsersData.AddRange(Users);
            context.AppFilesData.AddRange(FileDatas);
            context.AppFiles.AddRange(Files);
            context.SaveChanges();

        }

        public static byte[] fileContent = { 0xEF, 0xBB, 0xBF, 0x63, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x74 };

        public static List<AppUser> Users = new()
        {
            new AppUser { Id = Guid.NewGuid(), UserName = "User1", Email = "user1@mail.com", NormalizedEmail = "USER1@MAIL.COM"},
            new AppUser { Id = Guid.NewGuid(), UserName = "User2", Email = "user2@mail.com", NormalizedEmail = "USER2@MAIL.COM"},
            new AppUser { Id = Guid.NewGuid(), UserName = "User3", Email = "user3@mail.com", NormalizedEmail = "USER3@MAIL.COM"},
            new AppUser { Id = Guid.NewGuid(), UserName = "User4", Email = "user4@mail.com", NormalizedEmail = "USER4@MAIL.COM"},
        };

        public static List<AppFileData> FileDatas = new()
        {
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File1.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 10:00:00"),
                OwnerId = Users[0].Id, IsPublic = true},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File2.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 11:00:00"),
                OwnerId = Users[0].Id, IsPublic = false},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File3.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 12:00:00"),
                OwnerId = Users[1].Id, IsPublic = true},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File4.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 13:00:00"),
                OwnerId = Users[1].Id, IsPublic = false},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File5.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 14:00:00"),
                OwnerId = Users[2].Id, IsPublic = false},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File6.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 15:00:00"),
                OwnerId = Users[2].Id, IsPublic = false},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File7.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 16:00:00"),
                OwnerId = Users[3].Id, IsPublic = false, FileViewers = new[] { Users[0] } },
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File8.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 17:00:00"),
                OwnerId = Users[3].Id, IsPublic = false, FileViewers = new[] { Users[0] } },
        };

        public static List<AppFile> Files = new()
        {
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[0].Id},
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[1].Id},
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[2].Id},
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[3].Id},
        };
    }
}
