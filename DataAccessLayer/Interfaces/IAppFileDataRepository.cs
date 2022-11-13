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
        public Task<ICollection<AppFileData>> GetFilteredSortedPageByUser(int userId, QueryOptionsModel query);

        public Task<int> GetUserFilesCountAsync(int userId);
    }
}
