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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BuisnessLogicLayer.Services
{
    public class FileService : IFileService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        public FileService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<FileDataModel?> GetOwnByIdAsync(Guid userId, Guid id)
        {
            var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(id);
            if(fileData != null && fileData.OwnerId == userId)
            {
                return _mapper.Map<FileDataModel>(fileData);
            }
            return null;
        }

        //TODO: test it
        public async Task UpdateByUserAsync(Guid userId, FileDataModel model)
        {
            if (await _unitOfWork.AppFileDataRepository.IsOwner(model.Id, userId))
            {
                //TODO: Validate
                var fileData = _mapper.Map<AppFileData>(model);
                _unitOfWork.AppFileDataRepository.Update(fileData);
                await _unitOfWork.SaveAsync();
            }
            
        }

        public async Task<AppFileData?> GetFileByIdAsync(Guid userId, Guid fileId)
        {
            var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithContentAsync(fileId);
            if (fileData != null && fileData.OwnerId == userId)
            {
                return fileData;
            }
            return null;
        }

        public async Task<PaginationResultModel<FileDataModel>> GetUserFilesDataNoTrackingAsync(Guid userId, QueryModel query) 
        {
            int count = await _unitOfWork.AppFileDataRepository.GetUserFilesCountAsync(userId);
            var source = _unitOfWork.AppFileDataRepository.GetAllNoTraking().Where(fd => fd.OwnerId == userId);
            //var list = await this.TakePageFilteredAndOrderedAsync<AppFileData>(source, query);


            var result = await GetFilteredOrderedPaginatedAsync(source, query);
            result.TotalCount = count;

            return result;
            /*return new PaginationResultModel<FileDataModel>
            {
                TotalCount = count,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };*/
        }

        //method for admins only
        public async Task<PaginationResultModel<FileDataModel>> GetFilesDataAsync(QueryModel query)
        {
            int count = await _unitOfWork.AppFileDataRepository.GetFilesCountAsync();
            var source = _unitOfWork.AppFileDataRepository.GetAllNoTraking();
            /*if (query.SortColumn == "name") { 
                query.SortColumn = "UntrustedName";
            }
            var list = await this.TakePageFilteredAndOrderedAsync<AppFileData>(source, query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = count,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };*/

            var result = await GetFilteredOrderedPaginatedAsync(source, query);
            result.TotalCount = count;

            return result;
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
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize,
                    Data = new List<FileDataModel>()
                };
            }

            var source = (IQueryable<AppFileData>) user.ReadOnlyFiles;
            /*var list = await this.TakePageFilteredAndOrderedAsync<AppFileData>(source, query);

            return new PaginationResultModel<FileDataModel>
            {
                TotalCount = user.ReadOnlyFiles.Count,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Data = _mapper.Map<ICollection<FileDataModel>>(list)
            };*/

            var result = await GetFilteredOrderedPaginatedAsync(source, query);
            result.TotalCount = user.ReadOnlyFiles.Count;

            return result;
        }

        public async Task DeleteFileByIdAsync(FileDataModel fileDataModel)
        {
            var fileData = this._mapper.Map<AppFileData>(fileDataModel);
            _unitOfWork.AppFileDataRepository.Delete(fileData);
            await _unitOfWork.SaveAsync();
        }
        public async Task DeleteOwnAsync(Guid userId, Guid fileId)
        {
            //var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithContentAsync(fileId);
            var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(fileId);
            if (fileData != null && fileData.OwnerId == userId)
            {
                _unitOfWork.AppFileDataRepository.Delete(fileData);
                await _unitOfWork.SaveAsync();
            }
            //TODO: something with validation
        }

        public async Task<FileDataModel?> ShareByEmailAsync(Guid ownerId, string userEmail, Guid fileDataId)
        {
            if (String.IsNullOrEmpty(userEmail)) return null;
            var user = await _unitOfWork.AppUserRepository.GetByEmailAsync(userEmail);
            if (user == null) return null;
            var fileData = await _unitOfWork.AppFileDataRepository.GetByIdWithRelatedAsync(fileDataId);
            if(fileData == null) return null;
            if(fileData.OwnerId != ownerId) return null;

            fileData?.FileViewers?.Add(user);
            await _unitOfWork.SaveAsync();  
            return _mapper.Map<FileDataModel?>(fileData);
        }


        private async Task<PaginationResultModel<FileDataModel>> GetFilteredOrderedPaginatedAsync
            (IQueryable<AppFileData> source, QueryModel query)
        {
            if(!string.IsNullOrEmpty(query.FilterColumn) && !string.IsNullOrEmpty(query.FilterQuery))
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
    }
}
