using DataAccessLayer.Entities;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IAppUserRepository? _userRepository;
        private IAppFileDataRepository? _fileDataRepository;
        private IShortLinkRepository? _shortLinkRepository;

        public ILogger<UnitOfWork> Logger { get; set; }

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            Logger = logger;
        }

        public IAppFileDataRepository AppFileDataRepository => _fileDataRepository ??= new AppFileDataRepository(_context);

        public IAppUserRepository AppUserRepository => _userRepository ??= new AppUserRepository(_context);

        public IShortLinkRepository ShortLinkRepository => _shortLinkRepository ??= new ShortLinkRepository(_context);

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
