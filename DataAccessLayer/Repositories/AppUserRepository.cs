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
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(AppDbContext context) : base(context)
        {
        }

        internal AppUserRepository(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public async Task<AppUser?> GetByIdWithRelatedAsync(Guid userId)
        {
            return await Table.Include(u => u.AppFiles)
                .Include(u => u.ReadOnlyFiles)
                .ThenInclude(fd => fd.OwnerNav)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<int> GetReadOnlyFilesCountAsync(Guid userId)
        {
            var user = await Table.Include(u => u.ReadOnlyFiles)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.ReadOnlyFiles?.Count ?? 0;
        }

        public async Task<bool> IsExistByEmailAsync(string userEmail)
        {
            return await Table.Where(e => e.NormalizedEmail == userEmail.ToUpper()).AnyAsync();
        }

        public async Task<AppUser?> GetByEmailAsync(string userEmail)
        {
            return await Table.FirstOrDefaultAsync(u => u.NormalizedEmail == userEmail.ToUpper());
        }
    }
}
