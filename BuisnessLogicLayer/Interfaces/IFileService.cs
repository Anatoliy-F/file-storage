using BuisnessLogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Interfaces
{
    public interface IFileService
    {
        public Task<ShortFileDataModel?> GetShortFileDataAsync(Guid fileId);

        public Task<PaginationResultModel<FileDataModel>> GetUserFilesDataNoTrackingAsync(Guid userId, QueryModel query);

        public Task<PaginationResultModel<FileDataModel>> GetFilesDataAsync(QueryModel query);

        public Task<ICollection<FileDataModel>> GetAllFilesSharedWithUserAsync(Guid userId);

        public Task<PaginationResultModel<FileDataModel>>
           GetSharedWithUserFilesDataAsync(Guid userId, QueryModel query);

        public Task DeleteFileByIdAsync(Guid fileId);

        public Task<FileDataModel?> GetOwnByIdAsync(Guid userId, Guid id);

        public Task DeleteOwnByIdAsync(Guid userId, Guid fileId);
    }
}
