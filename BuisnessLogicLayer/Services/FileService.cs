using AutoMapper;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLogicLayer.Enums;
using DataAccessLayer.Exceptions;
using System.Diagnostics;

namespace BuisnessLogicLayer.Services
{
    /// <summary>
    /// Provides operations with file objects
    /// </summary>
    public class FileService : IFileService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        private const string DEFAULT_ERROR = "Something go wrong";

        /// <summary>
        /// Initialize new instance of FileService
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instanse, for access to repositories</param>
        /// <param name="mapper">mapper instanse for creating DTO's</param>
        public FileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<FileDataModel>> GetOwnByIdAsync(Guid userId, Guid id)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(id);

                if (fileData == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {id}"
                    };
                }
                if (fileData.OwnerId != userId)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = $"You do not own the file with: {id}"
                    };
                }
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<FileDataModel>(fileData)
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<FileDataModel>> GetSharedByIdAsync(Guid userId, Guid id)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(id);

                if (fileData == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {id}"
                    };
                }
                if (!fileData.IsPublic || fileData.FileViewers == null
                    || !fileData.FileViewers.Any(fv => fv.Id == userId))
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = $"You do not own the file with: {id}"
                    };
                }
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<FileDataModel>(fileData)
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<FileDataModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(id);

                if (fileData == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {id}"
                    };
                }
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<FileDataModel>(fileData)
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<bool>> UpdateOwnAsync(Guid userId, FileDataModel model)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(model.Id);
                if (fileData == null)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {model.Id}"
                    };
                }
                if (fileData.OwnerId != userId)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = $"You do not own the file with: {model.Id}"
                    };
                }

                //TODO: Validate

                fileData.UntrustedName = model.Name;
                fileData.Note = model.Note;
                fileData.IsPublic = model.IsPublic;

                if(model.Viewers.Count != 0)
                {
                    var viewers = new List<AppUser>();
                    foreach (var viewer in model.Viewers)
                    {
                        var user = await _unitOfWork.UserManager.FindByIdAsync(viewer.Id.ToString());
                        viewers.Add(user);
                    }
                    fileData.FileViewers = viewers;
                } else
                {
                    fileData.FileViewers = new List<AppUser>();
                }

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
        public async Task<ServiceResponse<bool>> UpdateAsync(FileDataModel model)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(model.Id);
                if (fileData == null)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {model.Id}"
                    };
                }
                //TODO: Validate

                fileData.UntrustedName = model.Name;
                fileData.Note = model.Note;
                fileData.IsPublic = model.IsPublic;

                if (model.Viewers.Count != 0)
                {
                    var viewers = new List<AppUser>();
                    foreach (var viewer in model.Viewers)
                    {
                        var user = await _unitOfWork.UserManager.FindByIdAsync(viewer.Id.ToString());
                        viewers.Add(user);
                    }
                    fileData.FileViewers = viewers;
                }
                else
                {
                    fileData.FileViewers = new List<AppUser>();
                }

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
        public async Task<ServiceResponse<FileDataModel>> GetOwnContentAsync(Guid userId, Guid fileId)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithContentAsync(fileId);

                if (fileData == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {fileId}"
                    };
                }

                if (fileData.OwnerId != userId)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = $"You do not own the file with: {fileId}"
                    };
                }

                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<FileDataModel>(fileData)
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<FileDataModel>> GetContentAsync(Guid fileId)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithContentAsync(fileId);

                if (fileData == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {fileId}"
                    };
                }
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<FileDataModel>(fileData)
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<PaginationResultModel<FileDataModel>>> GetAllOwnAsync(Guid userId, QueryModel query)
        {
            try
            {
                int count = await _unitOfWork.AppFileDataRepository.GetUserFilesCountAsync(userId);
                var source = _unitOfWork.AppFileDataRepository.GetAllNoTraking().Where(fd => fd.OwnerId == userId);

                var result = await GetFilteredOrderedPaginatedAsync(source, query);
                result.TotalCount = count;

                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = result
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<PaginationResultModel<FileDataModel>>> GetAllAsync(QueryModel query)
        {
            try
            {
                int count = await _unitOfWork.AppFileDataRepository.GetFilesCountAsync();
                var source = _unitOfWork.AppFileDataRepository.GetAllNoTraking();

                var result = await GetFilteredOrderedPaginatedAsync(source, query);
                result.TotalCount = count;

                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = result
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<PaginationResultModel<FileDataModel>>>
            GetSharedAsync(Guid userId, QueryModel query)
        {
            try
            {
                var user = await _unitOfWork.AppUserRepository.GetByIdWithRelatedAsync(userId);

                if (user == null)
                {
                    return new ServiceResponse<PaginationResultModel<FileDataModel>>
                    {
                        ResponseResult = ResponseResult.Error,
                        ErrorMessage = $"user with id {userId} doesn't exists"
                    };
                }

                if (user.ReadOnlyFiles.Count == 0)
                {
                    var emptyResult = new PaginationResultModel<FileDataModel>
                    {
                        TotalCount = user.ReadOnlyFiles.Count,
                        PageIndex = query.PageIndex,
                        PageSize = query.PageSize,
                        Data = new List<FileDataModel>()
                    };

                    return new ServiceResponse<PaginationResultModel<FileDataModel>>
                    {
                        ResponseResult = ResponseResult.Success,
                        Data = emptyResult
                    };
                }

                var source = _unitOfWork.AppFileDataRepository.GetShared(userId);

                var result = await GetFilteredOrderedPaginatedAsync(source, query);
                result.TotalCount = user.ReadOnlyFiles.Count;

                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = result
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaginationResultModel<FileDataModel>>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        public async Task<ServiceResponse<bool>> DeleteAsync(FileDataModel fileDataModel)
        {
            try
            {
                var fileData = this._mapper.Map<AppFileData>(fileDataModel);
                _unitOfWork.AppFileDataRepository.Delete(fileData);
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
        public async Task<ServiceResponse<bool>> DeleteOwnAsync(Guid userId, Guid fileId)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(fileId);

                if (fileData == null)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {fileId}"
                    };
                }

                if (fileData.OwnerId != userId)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = $"You do not own the file with: {fileId}"
                    };
                }

                _unitOfWork.AppFileDataRepository.Delete(fileData);
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
        public async Task<ServiceResponse<bool>> RefuseSharedAsync(Guid userId, Guid fileId)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(fileId);

                if (fileData == null)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with this id: {fileId}"
                    };
                }

                if (fileData.FileViewers == null || !fileData.FileViewers.Any(fv => fv.Id == userId))
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = $"You do not own the file with: {fileId}"
                    };
                }

                fileData.FileViewers = fileData.FileViewers.Where(fv => fv.Id != userId).ToList();
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
        public async Task<ServiceResponse<FileDataModel>> ShareByEmailAsync(Guid ownerId, string userEmail, Guid fileDataId)
        {
            if (!IsValidEmail(userEmail))
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = $"{userEmail} hasn't valid email format"
                };
            }

            try
            {
                var user = await _unitOfWork.AppUserRepository.GetByEmailAsync(userEmail);
                if (user == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No user with email {userEmail}"
                    };
                }

                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(fileDataId);
                if (fileData == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = $"No file with id: {fileDataId}"
                    };
                }

                if (fileData.OwnerId != ownerId)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = $"You do not own the file with: {fileDataId}"
                    };
                }

                fileData?.FileViewers?.Add(user);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<FileDataModel?>(fileData)
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }
        }

        /// <inheritdoc />
        private async Task<PaginationResultModel<FileDataModel>> GetFilteredOrderedPaginatedAsync
            (IQueryable<AppFileData> source, QueryModel query)
        {
            if (!string.IsNullOrEmpty(query.FilterColumn) && !string.IsNullOrEmpty(query.FilterQuery))
            {
                source = query.FilterColumn.ToLower() switch
                {
                    "name" => source.Where(e => e.UntrustedName.StartsWith(query.FilterQuery)),
                    "note" => source.Where(e => e.Note.StartsWith(query.FilterQuery)),
                    "ispublic" => source.Where(e => e.IsPublic == (query.FilterQuery.ToLower() == "true")),
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
                        source.OrderBy(e => e.UntrustedName) :
                        source.OrderByDescending(e => e.UntrustedName),
                    "note" => query.SortOrder == "ASC" ?
                        source.OrderBy(e => e.Note) :
                        source.OrderByDescending(e => e.Note),
                    "size" => query.SortOrder == "ASC" ?
                        source.OrderBy(e => e.Size) :
                        source.OrderByDescending(e => e.Size),
                    "uploaddatetime" => query.SortOrder == "ASC" ?
                        source.OrderBy(e => e.UploadDT) :
                        source.OrderByDescending(e => e.UploadDT),
                    "ispublic" => query.SortOrder == "ASC" ?
                        source.OrderBy(e => e.IsPublic) :
                        source.OrderByDescending(e => e.IsPublic),
                    _ => source
                };
            }

            source = source.Skip(query.PageIndex * query.PageSize).Take(query.PageSize);
            var list = await source.ToListAsync();

            return new PaginationResultModel<FileDataModel>
            {
                Data = _mapper.Map<ICollection<FileDataModel>>(list),
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                FilterColumn = query.FilterColumn,
                FilterQuery = query.FilterQuery,
                SortColumn = query.SortColumn,
                SortOrder = query.SortOrder,
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
