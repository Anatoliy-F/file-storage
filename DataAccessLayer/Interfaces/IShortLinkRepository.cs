using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IShortLinkRepository : IRepository<ShortLink>
    {
        public Task<bool> IsCollision(string link);

        public Task<bool> IsExist(Guid fileId);

        public Task<AppFileData?> GetFileContentByLinkAsync(string link);

        public Task<ShortLink?> GetShortLinkAsync(string link);

        public Task<AppFileData?> GetFileDataByLinkAsync(string link);

        public Task<ShortLink?> GetShortLinkWithRelatedAsync(string link);
    }
}
