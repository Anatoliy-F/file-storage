using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        public Task<AppUser?> FindByIdWithRelatedAsync(Guid userId);

        public Task<int> GetSharedWithUserFilesCountAsync(Guid userId);
    }
}
