using BuisnessLogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Interfaces
{
    public interface IUserService
    {
        public Task<ServiceResponse<bool>> IsExistByEmailAsync(string userEmail);

        public Task<ServiceResponse<UserModel>> GetByEmailAsync(string userEmail);
    }
}
