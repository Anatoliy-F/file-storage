using AutoMapper;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using BuisnessLogicLayer.Enums;
using DataAccessLayer.Exceptions;

namespace BuisnessLogicLayer.Services
{
    public class UserService : IUserService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        private const string DEFAULT_ERROR = "Something go wrong";

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> IsExistByEmailAsync(string userEmail)
        {
            if (IsValidEmail(userEmail))
            {
                try
                {
                    var exist = await _unitOfWork.AppUserRepository.IsExistByEmailAsync(userEmail);
                    if (exist)
                    {
                        return new ServiceResponse<bool>
                        {
                            ResponseResult = Enums.ResponseResult.Success,
                            Data = true
                        };
                    }
                    else
                    {
                        return new ServiceResponse<bool>
                        {
                            ResponseResult = Enums.ResponseResult.NotFound,
                            ErrorMessage = $"user with email: {userEmail}, isn't exist"
                        };
                    }
                }
                catch (CustomException ex)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.Error,
                        ErrorMessage = ex.Message
                    };
                }
                catch (Exception)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.Error,
                        ErrorMessage = DEFAULT_ERROR
                    };
                }
            }

            return new ServiceResponse<bool>
            {
                ResponseResult = ResponseResult.Error,
                ErrorMessage = $"{userEmail} hasn't valid email format"
            };
        }

        public async Task<ServiceResponse<UserModel>> GetByEmailAsync(string userEmail)
        {
            if (IsValidEmail(userEmail))
            {
                try
                {
                    var user = await _unitOfWork.AppUserRepository.GetByEmailAsync(userEmail);

                    if (user == null)
                    {
                        return new ServiceResponse<UserModel>
                        {
                            ResponseResult = ResponseResult.NotFound,
                            ErrorMessage = $"user with email: {userEmail}, isn't exist"
                        };
                    }
                    else
                    {
                        return new ServiceResponse<UserModel>
                        {
                            ResponseResult = ResponseResult.Success,
                            Data = _mapper.Map<UserModel>(user)
                        };
                    }
                }
                catch (CustomException ex)
                {
                    return new ServiceResponse<UserModel>
                    {
                        ResponseResult = ResponseResult.Error,
                        ErrorMessage = ex.Message
                    };
                }
                catch (Exception)
                {
                    return new ServiceResponse<UserModel>
                    {
                        ResponseResult = ResponseResult.Error,
                        ErrorMessage = DEFAULT_ERROR
                    };
                }
            }
            return new ServiceResponse<UserModel>
            {
                ResponseResult = Enums.ResponseResult.Error,
                ErrorMessage = $"{userEmail} hasn't valid email format"
            };
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
