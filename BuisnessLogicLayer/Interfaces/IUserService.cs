using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Interfaces
{
    public interface IUserService
    {
        public Task<bool> IsExistByEmailAsync(string userEmail);
    }
}
