using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        public async Task<AppFileData?> GetAppFileDataWithContentAsync(Guid id)
        {
            /*var data = await Table.FindAsync(id);
            if (data != null)
            {
                await Context.Entry(data).Reference(e => e.AppFileNav).LoadAsync();
                return data;
            }*/
            return await Table.Include(af => af.AppFileNav).FirstOrDefaultAsync(af => af.Id == id);
        }

        public async Task<ICollection<AppFileData>> GetFilteredSortedPageByUserAsync(Guid userId, QueryOptionsModel query)
        {
            var source = Table.Where(e => e.AppUserId == userId);
            return await TakePageFilteredAndOrdered(source, query);
        }

        public async Task<ICollection<AppFileData>> GetFilteredSortedWithUserDataAsync(QueryOptionsModel query)
        {
            var source = Table.Include(af => af.AppUserNav);
            return await TakePageFilteredAndOrdered(source, query);
        }

        public async Task<ICollection<AppFileData>> GetFilteredSortedSharedWithUserAsync(Guid userId, QueryOptionsModel query)
        {
            var source = Table.Include(fd => fd.FileViewers).Where(fd => fd.FileViewers.Any(u => u.Id == userId));
            return await TakePageFilteredAndOrdered(source, query);

        }

        public async Task<int> GetUserFilesCountAsync(Guid userId) => await Table.Where(e => e.AppUserId == userId).CountAsync();

        public async Task<int> GetFilesCountAsync() => await Table.CountAsync();
    }
}
