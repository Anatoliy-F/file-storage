using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Seeding database, using only in Development environment!
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// nitialize new instance of SeedController
        /// </summary>
        /// <param name="context">Persistence store</param>
        /// <param name="roleManager">Managing user roles in a persistence store</param>
        /// <param name="userManager">Provides the APIs for managing user in a persistence store.</param>
        /// <param name="env">Provides information about web hosting</param>
        public SeedController(AppDbContext context, RoleManager<IdentityRole<Guid>> roleManager, 
            UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
        }

        /// <summary>
        /// Seeding database fith test files
        /// </summary>
        /// <returns>Inserted files objects</returns>
        /// <exception cref="SecurityException">Throws if invoke in nin-development environment</exception>
        [HttpGet]
        public async Task<ActionResult> CreateFiles()
        {
            // prevents non-development environments from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");

            List<AppFileData> list = new();

                var user1 = await _userManager.FindByEmailAsync("user10@email.com");

                for(int i = 0; i < 50; i++)
                {
                    byte[] content = { 0xEF, 0xBB, 0xBF, 0x63, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x74, (byte)(0x63+i)};
                    var file = new AppFileData
                    {
                        AppFileNav = new AppFile { Content = content },
                        UntrustedName = $"file#{i}.txt",
                        Note = $"File Note #{i}. Something important",
                        Size = content.LongLength,
                        UploadDT = new DateTime(2022, 11, 1 + i/2),
                        OwnerId = user1.Id,
                        IsPublic = i % 3 == 0
                    };

                    _context.AppFilesData.Add(file);
                    list.Add(file);
                }

                _context.SaveChanges();

                var user2 = await _userManager.FindByEmailAsync("user12@email.com");

                for (int i = 0; i < 50; i++)
                {
                    byte[] content = { 0xEF, 0xBB, 0xBF, 0x63, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x74, (byte)(0x63 + i) };
                    var file = new AppFileData
                    {
                        AppFileNav = new AppFile { Content = content },
                        UntrustedName = $"file#{i}.txt",
                        Note = $"File Note #{i}. Something very important",
                        Size = content.LongLength,
                        UploadDT = new DateTime(2022, 11, 1 + i / 2),
                        OwnerId = user2.Id,
                        IsPublic = i % 3 == 0
                    };

                    _context.AppFilesData.Add(file);
                    list.Add(file);
                }

                _context.SaveChanges();

                var user3 = await _userManager.FindByEmailAsync("user14@email.com");

                for (int i = 0; i < 50; i++)
                {
                    byte[] content = { 0xEF, 0xBB, 0xBF, 0x63, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x74, (byte)(0x63 + i) };
                    var file = new AppFileData
                    {
                        AppFileNav = new AppFile { Content = content },
                        UntrustedName = $"file#{i}.txt",
                        Note = $"File Note #{i}. Something very important",
                        Size = content.LongLength,
                        UploadDT = new DateTime(2022, 11, 1 + i / 2),
                        OwnerId = user3.Id,
                        IsPublic = i%3 == 0
                    };

                    _context.AppFilesData.Add(file);
                    list.Add(file);
                }

                _context.SaveChanges();

            return new JsonResult(new
            {
                list.Count,
                Files = list.Select(fd => new
                {
                    Name = fd.UntrustedName!,
                    fd.Note,
                    fd.Size,
                    fd.UploadDT,
                    fd.OwnerId,
                    fd.IsPublic
                })
            });
        }

        /// <summary>
        /// Seeding database fith test users
        /// </summary>
        /// <returns>Inserted user objects</returns>
        /// <exception cref="SecurityException">Throws if invoke in nin-development environment</exception>
        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            // prevents non-development environments from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");

            // setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            // create the default roles (if they don't exist yet)
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
                await _roleManager.CreateAsync(new
                 IdentityRole<Guid>(role_RegisteredUser));

            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
                await _roleManager.CreateAsync(new
                 IdentityRole<Guid>(role_Administrator));

            // create a list to track the newly added users
            var addedUserList = new List<AppUser>();

            var email_Admin = "admin@email.com";
            if (await _userManager.FindByEmailAsync(email_Admin) == null)
            {
                // create a new admin ApplicationUser account
                var user_Admin = new AppUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "Admin",
                    Email = email_Admin,
                };

                // insert the admin user into the DB
                await _userManager.CreateAsync(user_Admin, "Sampl3Pa$$_Admin");

                // assign the "RegisteredUser" and "Administrator" roles
                await _userManager.AddToRoleAsync(user_Admin,
                 role_RegisteredUser);
                await _userManager.AddToRoleAsync(user_Admin,
                 role_Administrator);

                // confirm the e-mail and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                // add the admin user to the added users list
                addedUserList.Add(user_Admin);
            }


            for(int i = 0; i < 10; i++)
            {
                var emailForUser = $"user1{i}@email.com";

                if (await _userManager.FindByEmailAsync(emailForUser) == null)
                {
                    // create a new standard ApplicationUser account
                    var user_User = new AppUser()
                    {
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = $"user1{i}",
                        Email = emailForUser
                    };

                    // insert the standard user into the DB
                    await _userManager.CreateAsync(user_User, $"Sampl3Pa$$_User{i}");

                    // assign the "RegisteredUser" role
                    await _userManager.AddToRoleAsync(user_User,
                     role_RegisteredUser);

                    // confirm the e-mail and remove lockout
                    user_User.EmailConfirmed = true;
                    user_User.LockoutEnabled = false;

                    // add the standard user to the added users list
                    addedUserList.Add(user_User);
                    await _context.SaveChangesAsync();

                }
            }

            // if we added at least one user, persist the changes into the DB
            if (addedUserList.Count > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                addedUserList.Count,
                Users = addedUserList
            });
        }
    }
}
