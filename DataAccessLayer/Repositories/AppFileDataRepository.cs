using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
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

        public async Task<ICollection<AppFileData>> GetFilteredSortedPageByUser(int userId, int pageSize, int pageIndex, 
            string? sortColumn = null, string? sortOrder = null, string? filterColumn = null, 
            string? filterQuery = null)
        {
            var source = Table.Where(e => e.AppUserId == userId);
            return await TakePageFilteredAndOrdered(source, pageSize, pageIndex, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        public async Task<int> GetUserFilesCountAsync(int userId) => await Table.Where(e => e.AppUserId == userId).CountAsync();
    }
}
