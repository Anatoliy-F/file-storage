using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    /// <inheritdoc />
    public class AppFileDataRepository : BaseRepository<AppFileData>, IAppFileDataRepository
    {
        /// <summary>
        /// Initialize new instance of AppFileDataRepository
        /// </summary>
        /// <param name="context">AppDbContext instanse</param>
        public AppFileDataRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Contructor to interact with cli ef tools
        /// </summary>
        /// <param name="options">DbContextOptions<AppDbContext> options</param>
        internal AppFileDataRepository(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        /// <inheritdoc />
        public async Task<AppFileData?> GetByIdWithContentAsync(Guid id)
        {
            return await Table.Include(af => af.AppFileNav).FirstOrDefaultAsync(af => af.Id == id);
        }

        /// <inheritdoc />
        public async Task<AppFileData?> GetByIdWithRelatedAsync(Guid id)
        {
            return await Table.Include(af => af.OwnerNav)
                .Include(af => af.FileViewers)
                .Include(af => af.ShortLinkNav)
                .FirstOrDefaultAsync(af => af.Id == id);
        }

        /// <inheritdoc />
        public async Task<int> GetUserFilesCountAsync(Guid userId) => await Table.Where(e => e.OwnerId == userId).CountAsync();

        /// <inheritdoc />
        public async Task<int> GetFilesCountAsync() => await Table.CountAsync();

        /// <inheritdoc />
        public IQueryable<AppFileData> GetAllNoTraking() => Table.AsNoTracking();

        /// <inheritdoc />
        public IQueryable<AppFileData> GetShared(Guid userId) => Table.Include(fd => fd.FileViewers)
            .Where(f => f!.FileViewers!.Any(u => u.Id == userId));

        /// <inheritdoc />
        public override void Delete(AppFileData fileData)
        {
            Context.AppFilesData.Remove(fileData);
        }

        /// <inheritdoc />
        public async Task<bool> IsOwner(Guid fileId, Guid OwnerId) =>
            await Table.Where(e => e.Id == fileId && e.OwnerId == OwnerId).AnyAsync();

    }
}
