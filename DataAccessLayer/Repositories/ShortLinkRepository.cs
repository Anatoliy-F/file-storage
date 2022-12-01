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
    public class ShortLinkRepository : BaseRepository<ShortLink>, IShortLinkRepository
    {
        public ShortLinkRepository(AppDbContext context) : base(context)
        {
        }

        internal ShortLinkRepository(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public async Task<bool> IsExist(string link)
        {
            return await Table.AnyAsync(sl => sl.Link == link);
        }

        public async Task<bool> CanGenerate(Guid fileId)
        {
            return !(await Table.AnyAsync(sl => sl.AppFileDataId == fileId));
        }

        public async Task<AppFileData?> GetFileContetntByLinkAsync(string link)
        {
            var linkObj = await Table.Include(sl => sl.AppFileDataNav).ThenInclude(afd => afd.AppFileNav).FirstOrDefaultAsync(sl => sl.Link == link);
            return linkObj?.AppFileDataNav ?? null;
        }

        public Task<ShortLink?> GetShortLinkAsync(string link)
        {
            return Table.FirstOrDefaultAsync(sl => sl.Link == link);
        }
    }
}
