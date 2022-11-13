using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IAppFileDataRepository AppFileDataRepository { get; }
        IAppUserRepository AppUserRepository { get; }
        //UserManager<AppUser> UserManager { get; }
        
        Task SaveAsync();
    }
}
