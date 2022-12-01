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
        public Task<bool> IsExist(string link);

        public Task<bool> CanGenerate(Guid fileId);

        public Task<AppFileData?> GetFileContetntByLinkAsync(string link);

        public Task<ShortLink?> GetShortLinkAsync(string link);
    }
}
