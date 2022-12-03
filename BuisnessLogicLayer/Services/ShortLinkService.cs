using AutoMapper;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
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

        public ShortLinkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //public async Task<(bool IsSuccess, AppFileData? Data, string? ErrorMessage)> GetFileByShortLinkAsync(string link)
        public async Task<ServiceResponse<AppFileData>> GetFileByShortLinkAsync(string link)
        {
            var fileData = await _unitOfWork.ShortLinkRepository.GetFileContetntByLinkAsync(link);
            if(fileData == null || !fileData.IsPublic)
            {
                //return (false, null, INVALID_LINK);
                return new ServiceResponse<AppFileData>
                {
                    ErrorMessage = INVALID_LINK,
                };
            }
            //return (true, fileData, String.Empty);
            return new ServiceResponse<AppFileData>
            {
                IsSuccess = true,
                Data = fileData
            };
        }

        //public async Task<(bool IsSuccess, FileDataModel? Data, string? ErrorMessage)> DeleteLinkAsync(string link)
        public async Task<ServiceResponse<FileDataModel>> DeleteLinkAsync(string link)
        {
            var linkObj = await _unitOfWork.ShortLinkRepository.GetShortLinkAsync(link);
            
            if(linkObj == null)
            {
                //return (false, null, INVALID_LINK);
                return new ServiceResponse<FileDataModel>
                {
                    ErrorMessage = INVALID_LINK,
                };
            }

            //TODO: maybe request fileData first?
            var fileDataId = linkObj.AppFileDataId;
            _unitOfWork.ShortLinkRepository.Delete(linkObj);
            await _unitOfWork.SaveAsync();
            var fileData = await _unitOfWork.AppFileDataRepository.GetByIdAsync(fileDataId);
            //return (true, _mapper.Map<FileDataModel>(fileData), String.Empty);
            
            //TODO: should i return fileDataModel. Maybe no?
            return new ServiceResponse<FileDataModel> {
                IsSuccess = true,
                Data = _mapper.Map<FileDataModel>(fileData)
            };
        }

        //public async Task<(bool IsSuccess, ShortFileDataModel? Data, string? ErrorMessage)> GetShortFileDataAsync(string link)
        public async Task<ServiceResponse<ShortFileDataModel>> GetShortFileDataAsync(string link)
        {
            var fileData = await _unitOfWork.ShortLinkRepository.GetFileDataByLinkAsync(link);
            if(fileData == null)
            {
                //return (false, null, INVALID_LINK);
                return new ServiceResponse<ShortFileDataModel>
                {
                    ErrorMessage = INVALID_LINK,
                };
            }
            //return (true, _mapper.Map<ShortFileDataModel>(fileData), null);
            return new ServiceResponse<ShortFileDataModel> {
                IsSuccess = true,
                Data = _mapper.Map<ShortFileDataModel>(fileData)
            };
        }

        //public async Task<(bool IsSuccess, FileDataModel? Data, string? ErrorMessage)> GenerateForFileById(Guid fileId)
        public async Task<ServiceResponse<FileDataModel>> GenerateForFileByIdAsync(Guid fileId)
        {
            
            if(!(await _unitOfWork.ShortLinkRepository.CanGenerate(fileId)))
            {
                //return (false, null, "File with this [Id] already has ShortLink");
                return new ServiceResponse<FileDataModel> { 
                    ErrorMessage = "File with this [Id] already has ShortLink"
                };
            }
            
            var fileData = await _unitOfWork.AppFileDataRepository.GetByIdAsync(fileId);
            
            if(fileData == null)
            {
                //return (false, null, "There are no file with this [Id]");
                return new ServiceResponse<FileDataModel>
                {
                    ErrorMessage = "There are no file with this [Id]"
                };
            }

            if (!fileData.IsPublic)
            {
                //return (false, null, "If you want share this file via short link change it accessible level to \"public\"");
                return new ServiceResponse<FileDataModel>
                {
                    ErrorMessage = "If you want share this file via short link change it accessible level to \"public\""
                };
            }

            string shortUrl = string.Empty;

            for(int i = 0; i < 10; i++)
            {
                var byteArr = fileId.ToByteArray().Skip(i).Take(4).ToArray();
                shortUrl = WebEncoders.Base64UrlEncode(byteArr);
                
                if(!(await _unitOfWork.ShortLinkRepository.IsExist(shortUrl)))
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

            //return (true, _mapper.Map<FileDataModel>(fileData), String.Empty);
            return new ServiceResponse<FileDataModel> { 
                IsSuccess = true,
                Data = _mapper.Map<FileDataModel>(fileData),
            };
        }
    }
}
