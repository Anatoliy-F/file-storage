using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <inheritdoc />
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        /// <summary>
        /// Initialize new instance of AppUserRepository
        /// </summary>
        /// <param name="context">AppDbContext instanse</param>
        public AppUserRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Contructor to interact with cli ef tools
        /// </summary>
        /// <param name="options">DbContextOptions<AppDbContext> options</param>
        internal AppUserRepository(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <inheritdoc />
        public async Task<AppUser?> GetByIdWithRelatedAsync(Guid userId)
        {
            return await Table.Include(u => u.AppFiles)
                .Include(u => u.ReadOnlyFiles)
                .ThenInclude(fd => fd.OwnerNav)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <inheritdoc />
        public async Task<int> GetReadOnlyFilesCountAsync(Guid userId)
        {
            var user = await Table.Include(u => u.ReadOnlyFiles)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.ReadOnlyFiles?.Count ?? 0;
        }

        /// <inheritdoc />
        public async Task<int> GetUsersCountAsync() => await Table.CountAsync();

        /// <inheritdoc />
        public async Task<bool> IsExistByEmailAsync(string userEmail)
        {
            return await Table.Where(e => e.NormalizedEmail == userEmail.ToUpper()).AnyAsync();
        }

        /// <inheritdoc />
        public async Task<AppUser?> GetByEmailAsync(string userEmail)
        {
            return await Table.FirstOrDefaultAsync(u => u.NormalizedEmail == userEmail.ToUpper());
        }

        /// <inheritdoc />
        public IQueryable<AppUser> GetAllNoTracking() => Table.AsNoTracking();
    }
}
