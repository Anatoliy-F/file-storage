using AutoMapper;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Services
{
    public class FileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ShortFileDataModel?> GetShortFileDataAsync(Guid fileId)
        {
            var fileData = await _unitOfWork.AppFileDataRepository.FindByIdAsync(fileId);
            if (fileData != null && fileData.IsPublic)
            {
                return _mapper.Map<ShortFileDataModel>(fileData);
            }
            return null;
        }

        public async Task<PaginationResultModel<FileDataModel>> GetUserFilesDataAsync(Guid userId, PaginationRequestModel queryOptions) 
        {
            int count = await _unitOfWork.AppFileDataRepository.GetUserFilesCountAsync(userId);
            var query = _mapper.Map<QueryOptionsModel>(queryOptions);
            var list = await _unitOfWork.AppFileDataRepository.GetFilteredSortedPageByUserAsync(userId, query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = count,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };
        }

        //method for admins only
        public async Task<PaginationResultModel<FileDataModel>> GetFilesDataAsync(PaginationRequestModel queryOptions)
        {
            int count = await _unitOfWork.AppFileDataRepository.GetFilesCountAsync();
            var query = _mapper.Map<QueryOptionsModel>(queryOptions);
            var list = await _unitOfWork.AppFileDataRepository.GetFilteredSortedWithUserDataAsync(query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = count,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };
        }

        //get all files shared with user
        public async Task<ICollection<FileDataModel>> GetAllFilesSharedWithUserAsync(Guid userId)
        {
            var user = await _unitOfWork.AppUserRepository.FindByIdWithRelatedAsync(userId);
            var list = user?.ReadOnlyFiles ?? new List<AppFileData>();

            return _mapper.Map<ICollection<FileDataModel>>(list);
        }

        public async Task<PaginationResultModel<FileDataModel>> 
            GetSharedWithUserFilesDataAsync(Guid userId, PaginationRequestModel queryOptions)
        {
            var count = await _unitOfWork.AppUserRepository.GetSharedWithUserFilesCountAsync(userId);
            var query = _mapper.Map<QueryOptionsModel>(queryOptions);
            var list = _unitOfWork.AppFileDataRepository.GetFilteredSortedSharedWithUserAsync(userId, query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = count,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };
        }

        public async Task DeleteFileByIdAsync(Guid fileId)
        {
            await _unitOfWork.AppFileDataRepository.DeleteByIdAsync(fileId);
            await _unitOfWork.SaveAsync();
        }

        
    }
}
