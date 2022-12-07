using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.WebUtilities;
using AutoMapper;
using BuisnessLogicLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

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

        public static IMapper CreateMapperProfile()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return new Mapper(configuration);
        }

        public static UserManager<AppUser> TestUserManager<AppUser>(IUserStore<AppUser> store = null!) where AppUser : class
        {
            store ??= new Mock<IUserStore<AppUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<AppUser>>();
            var validator = new Mock<IUserValidator<AppUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<AppUser>>
            {
                new PasswordValidator<AppUser>()
            };
            var userManager = new UserManager<AppUser>(store, options.Object, new PasswordHasher<AppUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<AppUser>>>().Object);
            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<AppUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            return userManager;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole> store = null!) where TRole : class
        {
            store ??= new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>>
            {
                new RoleValidator<TRole>()
            };
            return new Mock<RoleManager<TRole>>(store, roles, MockLookupNormalizer(),
                new IdentityErrorDescriber(), null);
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        public static ILookupNormalizer MockLookupNormalizer()
        {
            var normalizerFunc = new Func<string, string>(i =>
            {
                if (i == null)
                {
                    return null!;
                }
                else
                {
                    return Convert.ToBase64String(Encoding.UTF8.GetBytes(i)).ToUpperInvariant();
                }
            });
            var lookupNormalizer = new Mock<ILookupNormalizer>();
            lookupNormalizer.Setup(i => i.NormalizeName(It.IsAny<string>())).Returns(normalizerFunc);
            lookupNormalizer.Setup(i => i.NormalizeEmail(It.IsAny<string>())).Returns(normalizerFunc);
            return lookupNormalizer.Object;
        }

        public static void SeedData(AppDbContext context)
        {
            context.AppUsersData.AddRange(Users);
            context.AppFilesData.AddRange(FileDatas);
            context.AppFiles.AddRange(Files);
            context.ShortLink.AddRange(ShortLinks);
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
                OwnerId = Users[2].Id, IsPublic = true},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File6.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 15:00:00"),
                OwnerId = Users[2].Id, IsPublic = false},
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File7.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 16:00:00"),
                OwnerId = Users[3].Id, IsPublic = true, FileViewers = new[] { Users[0] } },
            new AppFileData {Id = Guid.NewGuid(), UntrustedName = "File8.txt", Size = 11, UploadDT = DateTime.Parse("11/11/2022 17:00:00"),
                OwnerId = Users[3].Id, IsPublic = true, FileViewers = new[] { Users[0] } },
        };

        public static List<AppFile> Files = new()
        {
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[0].Id},
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[1].Id},
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[2].Id},
            new AppFile {Id = Guid.NewGuid(), Content = fileContent, AppFileDataId = FileDatas[3].Id},
        };

        public static List<ShortLink> ShortLinks = new()
        {
            new ShortLink { Id = Guid.NewGuid(), AppFileDataId = FileDatas[0].Id,
                Link = WebEncoders.Base64UrlEncode(FileDatas[0].Id.ToByteArray().Take(4).ToArray())},
            new ShortLink { Id = Guid.NewGuid(), AppFileDataId = FileDatas[7].Id,
                Link = WebEncoders.Base64UrlEncode(FileDatas[7].Id.ToByteArray().Take(4).ToArray())},
            //Inconsistent! FileData isn't public
            new ShortLink { Id = Guid.NewGuid(), AppFileDataId = FileDatas[3].Id,
                Link = WebEncoders.Base64UrlEncode(FileDatas[3].Id.ToByteArray().Take(4).ToArray())},
        };
    }
}
