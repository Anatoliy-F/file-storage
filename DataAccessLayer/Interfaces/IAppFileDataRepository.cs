using DataAccessLayer.Entities;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IAppFileDataRepository : IRepository<AppFileData>
    {
        public Task<ICollection<AppFileData>> GetFilteredSortedPageByUserAsync(Guid userId, QueryOptionsModel query);

        public Task<int> GetUserFilesCountAsync(Guid userId);

        public Task<AppFileData?> GetAppFileDataWithContentAsync(Guid id);

        public Task<int> GetFilesCountAsync();

        public Task<ICollection<AppFileData>> GetFilteredSortedSharedWithUserAsync(Guid userId, QueryOptionsModel query);

        public Task<ICollection<AppFileData>> GetFilteredSortedWithUserDataAsync(QueryOptionsModel query);
    }
}
