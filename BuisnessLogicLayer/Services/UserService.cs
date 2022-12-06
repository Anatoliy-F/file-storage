using AutoMapper;
using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace BuisnessLogicLayer.Services
{
    /// <inheritdoc />
    public class UserService : IUserService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        private const string DEFAULT_ERROR = "Something go wrong";

        /// <summary>
        /// Initialize new instance of UserService
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instanse, for access to repositories</param>
        /// <param name="mapper">mapper instanse for creating DTO's</param>
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <inheritdoc />
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
                            ResponseResult = ResponseResult.Success,
                            Data = true
                        };
                    }
                    else
                    {
                        return new ServiceResponse<bool>
                        {
                            ResponseResult = ResponseResult.NotFound,
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<ServiceResponse<PaginationResultModel<UserModel>>> GetAllAsync(QueryModel query)
        {
            try
            {
                int count = await _unitOfWork.AppUserRepository.GetUsersCountAsync();
                var source = _unitOfWork.AppUserRepository.GetAllNoTracking();

                var result = await GetFilteredOrderedPaginatedAsync(source, query);
                result.TotalCount = count;

                return new ServiceResponse<PaginationResultModel<UserModel>>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = result
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<PaginationResultModel<UserModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PaginationResultModel<UserModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<UserModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.AppUserRepository.GetByIdAsync(id);

                if (user == null)
                {
                    return new ServiceResponse<UserModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"user with id: {id}, isn't exist"
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

        /// <inheritdoc />
        public async Task<ServiceResponse<bool>> DeleteAsync(UserModel userModel)
        {
            try
            {
                var user = _mapper.Map<AppUser>(userModel);
                if(await _unitOfWork.UserManager.IsInRoleAsync(user, "Administrator"))
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = "You can't delete aministrator"
                    };
                }
                _unitOfWork.AppUserRepository.Delete(user);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse<bool>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = true
                };
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

        /// <inheritdoc />
        public async Task<ServiceResponse<bool>> UpdateAsync(UserModel userModel)
        {
            try
            {
                //Validation going on in UserManager
                var user = await _unitOfWork.UserManager.FindByIdAsync(userModel.Id.ToString());

                user.UserName = userModel.Name;
                user.Email = userModel.Email;
                

                await _unitOfWork.UserManager.UpdateAsync(user);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse<bool>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = true
                };
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

        private static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private async Task<PaginationResultModel<UserModel>> GetFilteredOrderedPaginatedAsync
           (IQueryable<AppUser> source, QueryModel query)
        {
            if (!string.IsNullOrEmpty(query.FilterColumn) && !string.IsNullOrEmpty(query.FilterQuery))
            {
                source = query.FilterColumn.ToLower() switch
                {
                    "name" => source.Where(e => e.UserName.StartsWith(query.FilterQuery)),
                    "email" => source.Where(e => e.Email.StartsWith(query.FilterQuery)),
                    _ => source
                };
            }

            if (!string.IsNullOrEmpty(query.SortColumn))
            {
                query.SortOrder = !string.IsNullOrEmpty(query.SortOrder) && query.SortOrder.ToUpper() == "ASC"
                    ? "ASC" : "DESC";

                source = query.SortColumn.ToLower() switch
                {
                    "name" => query.SortOrder == "ASC" ?
                        source.OrderBy(e => e.UserName) :
                        source.OrderByDescending(e => e.UserName),
                    "email" => query.SortOrder == "ASC" ?
                        source.OrderBy(e => e.Email) :
                        source.OrderByDescending(e => e.Email),
                    _ => source
                };
            }

            source = source.Skip(query.PageIndex * query.PageSize).Take(query.PageSize);
            var list = await source.ToListAsync();

            return new PaginationResultModel<UserModel>
            {
                Data = _mapper.Map<ICollection<UserModel>>(list),
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                FilterColumn = query.FilterColumn,
                FilterQuery = query.FilterQuery,
                SortColumn = query.SortColumn,
                SortOrder = query.SortOrder,
            };
        }
    }
}
