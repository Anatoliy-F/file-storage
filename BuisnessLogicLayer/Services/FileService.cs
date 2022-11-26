using AutoMapper;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BuisnessLogicLayer.Services
{
    public class FileService : BaseService
    {
        public FileService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<ShortFileDataModel?> GetShortFileDataAsync(Guid fileId)
        {
            var fileData = await _unitOfWork.AppFileDataRepository.GetByIdAsync(fileId);
            if (fileData != null && fileData.IsPublic)
            {
                return _mapper.Map<ShortFileDataModel>(fileData);
            }
            return null;
        }

        public async Task<PaginationResultModel<FileDataModel>> GetUserFilesDataNoTrackingAsync(Guid userId, QueryModel query) 
        {
            int count = await _unitOfWork.AppFileDataRepository.GetUserFilesCountAsync(userId);
            var source = _unitOfWork.AppFileDataRepository.GetAllNoTraking().Where(fd => fd.OwnerId == userId);
            var list = this.TakePageFilteredAndOrdered<AppFileData>(source, query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = count,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };
        }

        //method for admins only
        public async Task<PaginationResultModel<FileDataModel>> GetFilesDataAsync(QueryModel query)
        {
            int count = await _unitOfWork.AppFileDataRepository.GetFilesCountAsync();
            var source = _unitOfWork.AppFileDataRepository.GetAllNoTraking();
            var list = this.TakePageFilteredAndOrdered<AppFileData>(source, query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = count,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };
        }

        //get all files shared with user
        public async Task<ICollection<FileDataModel>> GetAllFilesSharedWithUserAsync(Guid userId)
        {
            var user = await _unitOfWork.AppUserRepository.GetByIdWithRelatedAsync(userId);
            var list = user?.ReadOnlyFiles ?? new List<AppFileData>();

            return _mapper.Map<ICollection<FileDataModel>>(list);
        }

        public async Task<PaginationResultModel<FileDataModel>> 
            GetSharedWithUserFilesDataAsync(Guid userId, QueryModel query)
        {
            var user = await _unitOfWork.AppUserRepository.GetByIdWithRelatedAsync(userId) ?? new AppUser();
            
            if(user.ReadOnlyFiles.Count == 0)
            {
                return new PaginationResultModel<FileDataModel>
                {
                    TotalCount = user.ReadOnlyFiles.Count,
                    Data = new List<FileDataModel>()
                };
            }

            var source = (IQueryable<AppFileData>) user.ReadOnlyFiles;
            var list = this.TakePageFilteredAndOrdered<AppFileData>(source, query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = user.ReadOnlyFiles.Count,
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
