using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Interfaces
{
    /// <summary>
    /// Manage repositories life cycle
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Return instance of AppFileDataRepository
        /// </summary>
        IAppFileDataRepository AppFileDataRepository { get; }

        /// <summary>
        /// Return instance of AppUserRepository
        /// </summary>
        IAppUserRepository AppUserRepository { get; }

        /// <summary>
        /// Return instance of ShortLinkRepository
        /// </summary>
        IShortLinkRepository ShortLinkRepository { get; }

        /// <summary>
        /// Return instance of Identity UserManager
        /// </summary>
        public UserManager<AppUser> UserManager { get; }

        /// <summary>
        /// Return instance of Identity RoleManager
        /// </summary>
        public RoleManager<IdentityRole<Guid>> RoleManager { get; }

        /// <summary>
        /// Perform saving operations
        /// </summary>
        /// <returns>void</returns>
        Task SaveAsync();
    }
}
