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

        public async Task<ICollection<AppFileData>> GetFilteredSortedPageByUser(Guid userId, QueryOptionsModel query)
        {
            var source = Table.Where(e => e.AppUserId == userId);
            return await TakePageFilteredAndOrdered(source, query);
        }

        public async Task<int> GetUserFilesCountAsync(Guid userId) => await Table.Where(e => e.AppUserId == userId).CountAsync();
    }
}
