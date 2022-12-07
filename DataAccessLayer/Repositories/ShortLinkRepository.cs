using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    /// <inheritdoc />
    public class ShortLinkRepository : BaseRepository<ShortLink>, IShortLinkRepository
    {
        /// <summary>
        /// Initialize new instance of ShortLinkRepository
        /// </summary>
        /// <param name="context">AppDbContext instanse</param>
        public ShortLinkRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Contructor to interact with cli ef tools
        /// </summary>
        /// <param name="options">DbContextOptions<AppDbContext> options</param>
        internal ShortLinkRepository(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <inheritdoc />
        public async Task<bool> IsCollision(string link)
        {
            return await Table.AnyAsync(sl => sl.Link == link);
        }

        /// <inheritdoc />
        public async Task<bool> IsExist(Guid fileId)
        {
            return await Table.AnyAsync(sl => sl.AppFileDataId == fileId);
        }

        /// <inheritdoc />
        public async Task<AppFileData?> GetFileContentByLinkAsync(string link)
        {
            var linkObj = await Table.Include(sl => sl.AppFileDataNav).ThenInclude(afd => afd!.AppFileNav).FirstOrDefaultAsync(sl => sl.Link == link);
            return linkObj?.AppFileDataNav ?? null;
        }

        /// <inheritdoc />
        public async Task<AppFileData?> GetFileDataByLinkAsync(string link)
        {
            var linkObj = await Table.Include(sl => sl.AppFileDataNav).FirstOrDefaultAsync(sl => sl.Link == link);
            return linkObj?.AppFileDataNav ?? null;
        }

        /// <inheritdoc />
        public async Task<ShortLink?> GetShortLinkAsync(string link)
        {
            return await Table.FirstOrDefaultAsync(sl => sl.Link == link);
        }

        /// <inheritdoc />
        public async Task<ShortLink?> GetShortLinkWithRelatedAsync(string link)
        {
            return await Table.Include(sl => sl.AppFileDataNav).FirstOrDefaultAsync(sl => sl.Link == link);
        }
    }
}
