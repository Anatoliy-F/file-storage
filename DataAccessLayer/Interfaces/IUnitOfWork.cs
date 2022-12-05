using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IAppFileDataRepository AppFileDataRepository { get; }
        
        IAppUserRepository AppUserRepository { get; }
        
        IShortLinkRepository ShortLinkRepository { get; }
        public UserManager<AppUser> UserManager { get; }

        public RoleManager<IdentityRole<Guid>> RoleManager { get; }

        Task SaveAsync();
    }
}
