using AutoMapper;
using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Exceptions;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public class ShortLinkService : IShortLinkService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        private const string INVALID_LINK = "Invalid link";
        private const string DEFAULT_ERROR = "Something go wrong";

        public ShortLinkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<FileDataModel>> GetFileByShortLinkAsync(string link)
        {
            try
            {
                var fileData = await _unitOfWork.ShortLinkRepository.GetFileContentByLinkAsync(link);

                if (fileData == null)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = INVALID_LINK,
                    };
                }

                if (!fileData.IsPublic)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = INVALID_LINK,
                    };
                }

                if (fileData.AppFileNav == null || fileData.AppFileNav.Content.Length == 0)
                {
                    return new ServiceResponse<FileDataModel>
                    {
                        ResponseResult = ResponseResult.Error,
                        ErrorMessage = "No file content",
                    };
                }

                return new ServiceResponse<FileDataModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<FileDataModel>(fileData),
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

        public async Task<ServiceResponse<bool>> DeleteLinkAsync(string link)
        {
            try
            {
                var linkObj = await _unitOfWork.ShortLinkRepository.GetShortLinkWithRelatedAsync(link);

                if (linkObj == null)
                {
                    return new ServiceResponse<bool>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = INVALID_LINK,
                    };
                }

                _unitOfWork.ShortLinkRepository.Delete(linkObj);
                await _unitOfWork.SaveAsync();

                return new ServiceResponse<bool>
                {
                    ResponseResult = Enums.ResponseResult.Success,
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


        public async Task<ServiceResponse<ShortFileDataModel>> GetShortFileDataAsync(string link)
        {
            try
            {
                var fileData = await _unitOfWork.ShortLinkRepository.GetFileDataByLinkAsync(link);
                if (fileData == null)
                {
                    return new ServiceResponse<ShortFileDataModel>
                    {
                        ResponseResult = Enums.ResponseResult.NotFound,
                        ErrorMessage = INVALID_LINK,
                    };
                }

                if (!fileData.IsPublic)
                {
                    return new ServiceResponse<ShortFileDataModel>
                    {
                        ResponseResult = Enums.ResponseResult.AccessDenied,
                        ErrorMessage = INVALID_LINK,
                    };
                }

                return new ServiceResponse<ShortFileDataModel>
                {
                    ResponseResult = Enums.ResponseResult.Success,
                    Data = _mapper.Map<ShortFileDataModel>(fileData)
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<ShortFileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<ShortFileDataModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            } 
        }

        public async Task<ServiceResponse<ShortLinkModel>> GenerateForFileByIdAsync(Guid fileId)
        {
            try
            {
                var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(fileId);

                if (fileData == null)
                {
                    return new ServiceResponse<ShortLinkModel>
                    {
                        ResponseResult = ResponseResult.NotFound,
                        ErrorMessage = "There are no file with this [Id]"
                    };
                }

                if (fileData.ShortLinkNav != null)
                {
                    return new ServiceResponse<ShortLinkModel>
                    {
                        ResponseResult = ResponseResult.Error,
                        ErrorMessage = "File with this [Id] already has ShortLink"
                    };
                }

                if (!fileData.IsPublic)
                {
                    return new ServiceResponse<ShortLinkModel>
                    {
                        ResponseResult = ResponseResult.AccessDenied,
                        ErrorMessage = "If you want share this file via short link change it accessible level to \"public\""
                    };
                }

                string shortUrl = string.Empty;

                for (int i = 0; i < 10; i++)
                {
                    var byteArr = fileId.ToByteArray().Skip(i).Take(4).ToArray();
                    shortUrl = WebEncoders.Base64UrlEncode(byteArr);

                    if (!(await _unitOfWork.ShortLinkRepository.IsCollision(shortUrl)))
                    {
                        break;
                    }
                }

                ShortLink link = new()
                {
                    Link = shortUrl,
                    AppFileDataId = fileId,
                };

                fileData.ShortLinkNav = link;

                await _unitOfWork.SaveAsync();

                return new ServiceResponse<ShortLinkModel>
                {
                    ResponseResult = ResponseResult.Success,
                    Data = _mapper.Map<ShortLinkModel>(link),
                };
            }
            catch (CustomException ex)
            {
                return new ServiceResponse<ShortLinkModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception)
            {
                return new ServiceResponse<ShortLinkModel>
                {
                    ResponseResult = ResponseResult.Error,
                    ErrorMessage = DEFAULT_ERROR
                };
            }  
        }
    }
}
