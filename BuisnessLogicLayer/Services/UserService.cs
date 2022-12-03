using AutoMapper;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public class UserService : IUserService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> IsExistByEmailAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return false;
            }
            return await _unitOfWork.AppUserRepository.IsExistByEmailAsync(userEmail);
        }

        public async Task<UserModel?> GetByEmailAsync(string userEmail)
        {
            var user = await _unitOfWork.AppUserRepository.GetByEmailAsync(userEmail);
            if(user == null)
            {
                return null;
            }
            return _mapper.Map<UserModel>(user);
        }
    }
}
