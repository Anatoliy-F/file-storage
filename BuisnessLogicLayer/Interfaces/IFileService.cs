using BuisnessLogicLayer.Models;

namespace BuisnessLogicLayer.Interfaces
{
    /// <summary>
    /// Provides operations with file objects
    /// </summary>
    public interface IFileService
    {
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

        /// <summary>
        /// Return paginated user's read-only FileDataModel objects (not owned)
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="query">QueryModel (Pagination, sort, filter info)</param>
        /// <returns>PaginationResultModel<FileDataModel></returns>
        public Task<ServiceResponse<PaginationResultModel<FileDataModel>>>
           GetSharedAsync(Guid userId, QueryModel query);

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

        /// <summary>
        /// Return FileDataModel with file content using owning check
        /// </summary>
        /// <param name="userId">Owner Id</param>
        /// <param name="fileId">FileData id</param>
        /// <returns></returns>
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
        /// Return file metadata with related information without owning check
        /// For administrators only!
        /// </summary>
        /// <param name="id">File id</param>
        /// <returns>FileDataModel object with owner and viewers collection</returns>
        public Task<ServiceResponse<FileDataModel>> GetByIdAsync(Guid id);

        /// <summary>
        /// Delete file without owning check
        /// For administrators only!
        /// </summary>
        /// <param name="fileDataModel">FileDataModel instanse</param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeleteAsync(FileDataModel fileDataModel);

        /// <summary>
        /// Return filedata with content without owning check
        /// For administrators only!
        /// </summary>
        /// <param name="fileId">FileData id</param>
        /// <returns>FileDataModel instanse with content</returns>
        public Task<ServiceResponse<FileDataModel>> GetContentAsync(Guid fileId);

        /// <summary>
        /// Update file metadata without owning check
        /// For administrators only!
        /// </summary>
        /// <param name="model"></param>
        /// <returns>FileDataModel instanse</returns>
        public Task<ServiceResponse<bool>> UpdateAsync(FileDataModel model);


        /// <summary>
        /// Return fileData shared with user by file Id with view access check
        /// </summary>
        /// <param name="userId">Id of the user who has access to the file</param>
        /// <param name="id">File id</param>
        /// <returns>FielDataModel instanse with related data, without content</returns>
        public Task<ServiceResponse<FileDataModel>> GetSharedByIdAsync(Guid userId, Guid id);

        /// <summary>
        /// Denies granted access to a file
        /// </summary>
        /// <param name="userId">Id of the user who has access to the file</param>
        /// <param name="fileId">File id</param>
        /// <returns>TRUE access denied, FALSE otherwise</returns>
        public Task<ServiceResponse<bool>> RefuseSharedAsync(Guid userId, Guid fileId);

        /// <summary>
        /// Create and persist file with metadata
        /// </summary>
        /// <param name="fileName">File name (untrusted form upload)</param>
        /// <param name="note">Comment to file</param>
        /// <param name="ownerId">Id of user who upload file</param>
        /// <param name="content">byte[] array with file content</param>
        /// <returns>AppFileData model</returns>
        public Task<ServiceResponse<FileDataModel>> AddFromScratch(string fileName,
            string note, Guid ownerId, byte[] content);
    }
}
