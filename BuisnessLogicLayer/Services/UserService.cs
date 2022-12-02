using AutoMapper;
using BuisnessLogicLayer.Interfaces;
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

        
    }
}
