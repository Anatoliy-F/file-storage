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

        /*public async Task<IQueryable<AppFileData>> GetFilteredSortedPageByUserAsync(Guid userId, QueryOptionsModel query)
        {
            var source = Table.Where(e => e.OwnerId == userId);
            return source.ToList();
            //return await TakePageFilteredAndOrdered(source, query);
        }*/

       /* public async Task<ICollection<AppFileData>> GetFilteredSortedWithUserDataAsync(QueryOptionsModel query)
        {
            var source = Table.Include(af => af.OwnerNav);
            return source.ToList();
            //return await TakePageFilteredAndOrdered(source, query);
        }*/

        /*public async Task<ICollection<AppFileData>> GetFilteredSortedSharedWithUserAsync(Guid userId, QueryOptionsModel query)
        {
            var source = Table.Include(fd => fd.FileViewers).Where(fd => fd.FileViewers.Any(u => u.Id == userId));
            return source.ToList();
            //return await TakePageFilteredAndOrdered(source, query);

        }*/

        public async Task<int> GetUserFilesCountAsync(Guid userId) => await Table.Where(e => e.OwnerId == userId).CountAsync();

        public async Task<int> GetFilesCountAsync() => await Table.CountAsync();

        public IQueryable<AppFileData> GetAllNoTraking() => Table.AsNoTracking();
        
    }
}
