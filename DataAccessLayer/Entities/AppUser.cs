using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Entities
{
    /// <summary>
    /// Application User Entity. Inherited from IdentityUser<Guid>
    /// </summary>
    public class AppUser : IdentityUser<Guid>, IEntity
    {
        /// <summary>
        /// Collection of user files
        /// </summary>
        public ICollection<AppFileData> AppFiles { get; set; } = new List<AppFileData>();

        /// <summary>
        /// Collection of files to which the user has been granted access
        /// </summary>
        public ICollection<AppFileData> ReadOnlyFiles { get; set; } = new List<AppFileData>();
    }
}
