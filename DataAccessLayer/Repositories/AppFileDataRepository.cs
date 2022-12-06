using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class AppFileDataRepository : BaseRepository<AppFileData>, IAppFileDataRepository
    {
        public AppFileDataRepository(AppDbContext context) : base(context)
        {
        }

        internal AppFileDataRepository(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }
            
        public async Task<AppFileData?> GetByIdWithContentAsync(Guid id)
        {
            return await Table.Include(af => af.AppFileNav).FirstOrDefaultAsync(af => af.Id == id);
        }

        public async Task<AppFileData?> GetByIdWithRelatedAsync(Guid id)
        {
            return await Table.Include(af => af.OwnerNav)
                .Include(af => af.FileViewers)
                .Include(af => af.ShortLinkNav)
                .FirstOrDefaultAsync(af => af.Id == id);
        }

        public async Task<int> GetUserFilesCountAsync(Guid userId) => await Table.Where(e => e.OwnerId == userId).CountAsync();

        public async Task<int> GetFilesCountAsync() => await Table.CountAsync();

        public IQueryable<AppFileData> GetAllNoTraking() => Table.AsNoTracking();

        public IQueryable<AppFileData> GetShared(Guid userId) => Table
            .Where(f => f.FileViewers != null && f.FileViewers.Any(u => u.Id == userId));

        public override void Delete(AppFileData fileData)
        {
            Context.AppFilesData.Remove(fileData);
        }

        public async Task<bool> IsOwner(Guid fileId, Guid OwnerId) =>
            await Table.Where(e => e.Id == fileId && e.OwnerId == OwnerId).AnyAsync();

    }
}
