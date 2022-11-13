using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IAppFileDataRepository : IRepository<AppFileData>
    {
        public Task<ICollection<AppFileData>> GetFilteredSortedPageByUser(int userId, int pageSize, int pageIndex,
            string? sortColumn = null, string? sortOrder = null,
            string? filterColumn = null, string? filterQuery = null);

        public Task<int> GetUserFilesCountAsync(int userId);
    }
}
