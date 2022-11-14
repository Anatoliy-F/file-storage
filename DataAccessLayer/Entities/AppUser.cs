using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Entities
{
    public class AppUser : IdentityUser<Guid>, IEntity
    {
        public ICollection<AppFileData> AppFiles { get; set; } = new List<AppFileData>();
        public ICollection<AppFileData> ReadOnlyFiles { get; set; } = new List<AppFileData>();
    }
}
