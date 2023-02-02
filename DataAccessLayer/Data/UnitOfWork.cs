using DataAccessLayer.Entities;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Data
{
    /// <inheritdoc />
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IAppUserRepository? _userRepository;
        private IAppFileDataRepository? _fileDataRepository;
        private IShortLinkRepository? _shortLinkRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public ILogger<UnitOfWork> Logger { get; set; }

        /// <summary>
        /// Initialize new instance of UnitOfWork
        /// </summary>
        /// <param name="context">AppDbContext instanse</param>
        /// <param name="logger">Logger instanse</param>
        /// <param name="userManager">Identity UserManager instanse</param>
        /// <param name="roleManager">Identity role manager instanse</param>
        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger, 
            UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            Logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <inheritdoc />
        public IAppFileDataRepository AppFileDataRepository => _fileDataRepository ??= new AppFileDataRepository(_context);

        /// <inheritdoc />
        public IAppUserRepository AppUserRepository => _userRepository ??= new AppUserRepository(_context);

        /// <inheritdoc />
        public IShortLinkRepository ShortLinkRepository => _shortLinkRepository ??= new ShortLinkRepository(_context);

        /// <inheritdoc />
        public UserManager<AppUser> UserManager => _userManager;

        /// <inheritdoc />
        public RoleManager<IdentityRole<Guid>> RoleManager => _roleManager;

        /// <inheritdoc />
        public async Task SaveAsync()
        {
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Logger.LogError(ex, "Concurrency broken :(");
                throw new CustomConcurrencyException("A concurrency error happened.", ex);
            }
            catch (RetryLimitExceededException ex)
            {
                Logger.LogError(ex, "SQL Server broken :(");
                throw new CustomRetryLimitExceededException("There is a problem with SQL Server.", ex);
            }
            catch (DbUpdateException ex)
            {
                Logger.LogError(ex, "Update in trouble :(");
                throw new CustomDbUpdateException("An error occurred updating the database", ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Something went wrong :(");
                throw new CustomException("An error occurred updating the database", ex);
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
