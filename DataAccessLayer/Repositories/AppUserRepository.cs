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

        public async Task<AppUser?> FindByIdWithRelatedAsync(Guid userId)
        {
            return await Table.Include(u => u.AppFiles)
                .Include(u => u.ReadOnlyFiles)
                .ThenInclude(fd => fd.AppUserNav)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<int> GetSharedWithUserFilesCountAsync(Guid userId)
        {
            return await Table.Include(u => u.ReadOnlyFiles).CountAsync();
        }
    }
}
