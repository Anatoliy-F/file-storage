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

        public override void Delete(AppFileData fileData)
        {
            if(fileData.AppFileNav != null)
            {
                Context.AppFiles.Remove(fileData.AppFileNav);
            }
            else
            {
                Context.Entry(new AppFile { Id = fileData.AppFileId}).State = EntityState.Deleted;
            }
            Context.AppFilesData.Remove(fileData);
        }

        public override async Task DeleteByIdAsync(Guid id)
        {
            /*var fileData = await this.GetByIdAsync(id);
            if(fileData != null)
            {
                Context.Entry(new AppFile { Id = fileData.AppFileId }).State = EntityState.Deleted;
            }*/
            if(Table.Local.Any(fd => fd.Id == id))
            {
                Table.Remove(Table.Local.First(fd => fd.Id == id));
            }
            else
            {
                Context.Entry(new AppFileData { Id = id }).State = EntityState.Deleted;
            }
        }

    }
}
