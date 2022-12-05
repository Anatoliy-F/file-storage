using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Interfaces
{
    public interface IFileService
    {
        /*/// <summary>
        /// Return ShortFileDataObject, with properties
        /// Id, Name, Note, Size, UploadDateTime
        /// using for responses via short link for public accessed files
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <returns>ShortFileDataObject</returns>
        public Task<ShortFileDataModel?> GetShortFileDataAsync(Guid fileId);*/


        /// <summary>
        /// Returns PaginationResultModel (properties for pagination on client side)
        /// with FileDataModel collections by owner
        /// Retrieved by EF as no tracked, fo readonly purposes
        /// </summary>
        /// <param name="userId">Files owner Id</param>
        /// <param name="query">QueryModel (Pagination, sort, filter info)</param>
        /// <returns>PaginationResultModel<FileDataModel></returns>
        public Task<ServiceResponse<PaginationResultModel<FileDataModel>>> GetAllOwnAsync(Guid userId, QueryModel query);

        /// <summary>
        /// Returns PaginationResultModel (properties for pagination on client side)
        /// with FileDataModel collections
        /// Used by ADMINISTRATORS
        /// </summary>
        /// <param name="query">QueryModel (Pagination, sort, filter info)</param>
        /// <returns>PaginationResultModel<FileDataModel></returns>
        public Task<ServiceResponse<PaginationResultModel<FileDataModel>>> GetAllAsync(QueryModel query);

        /*/// <summary>
        /// Return all user's read-only FileDataModel objects (not owned) 
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Read-only FileData collection</returns>
        public Task<ICollection<FileDataModel>> GetAllFilesSharedWithUserAsync(Guid userId);*/

        /// <summary>
        /// Return paginated user's read-only FileDataModel objects (not owned)
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="query">QueryModel (Pagination, sort, filter info)</param>
        /// <returns>PaginationResultModel<FileDataModel></returns>
        public Task<ServiceResponse<PaginationResultModel<FileDataModel>>>
           GetSharedAsync(Guid userId, QueryModel query);

        //TODO: make deccision about delete by id
        /*public Task DeleteAsync(Guid fileId);*/

        /// <summary>
        /// Return FileDataModel object by FileId and OwnerId
        /// </summary>
        /// <param name="userId">OwnerId</param>
        /// <param name="id">FileId</param>
        /// <returns>FileDataModel</returns>
        public Task<ServiceResponse<FileDataModel>> GetOwnByIdAsync(Guid userId, Guid id);

        /// <summary>
        /// Delete FileDataModel object by FileId and OwnerId
        /// </summary>
        /// <param name="userId">OwnerId</param>
        /// <param name="fileId">FileId</param>
        /// <returns>Void</returns>
        public Task<ServiceResponse<bool>> DeleteOwnAsync(Guid userId, Guid fileId);

        public Task<ServiceResponse<FileDataModel>> GetOwnContentAsync(Guid userId, Guid fileId);

        /// <summary>
        /// Update AppFileData with ownership validation
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> UpdateOwnAsync(Guid userId, FileDataModel model);

        /// <summary>
        /// Share file with user by email
        /// </summary>
        /// <param name="ownerId">File owner id</param>
        /// <param name="userEmail">User id</param>
        /// <param name="fileDataId">FileData id</param>
        /// <returns></returns>
        public Task<ServiceResponse<FileDataModel>> ShareByEmailAsync(Guid ownerId, string userEmail, Guid fileDataId);

        /// <summary>
        /// Method for ADMINS
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ServiceResponse<FileDataModel>> GetByIdAsync(Guid id);

        /// <summary>
        /// Method for ADMINS
        /// </summary>
        /// <param name="fileDataModel"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeleteAsync(FileDataModel fileDataModel);

        /// <summary>
        /// Method for ADMINS
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<ServiceResponse<FileDataModel>> GetContentAsync(Guid fileId);

        /// <summary>
        /// Method for ADMINS
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> UpdateAsync(FileDataModel model);

        public Task<ServiceResponse<FileDataModel>> GetSharedByIdAsync(Guid userId, Guid id);
    }
}
