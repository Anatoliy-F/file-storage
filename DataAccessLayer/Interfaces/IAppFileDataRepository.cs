using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    /// <summary>
    /// Describe operations with AppFileData objects (contains file metadata)
    /// </summary>
    public interface IAppFileDataRepository : IRepository<AppFileData>
    {
        /// <summary>
        /// Get all entities (without related data), result should be filtered, ordered and paginated
        /// No tracked, for readonly usage
        /// </summary>
        /// <returns>Collection for read-only usage</returns>
        public IQueryable<AppFileData> GetAllNoTraking();

        /// <summary>
        /// Returns the number of files the user owns
        /// </summary>
        /// <param name="userId">Files owner id</param>
        /// <returns>Total users files count</returns>
        public Task<int> GetUserFilesCountAsync(Guid userId);

        /// <summary>
        /// Get AppFileData object by id, with AppFile content (byte[])
        /// </summary>
        /// <param name="id">Id of AppFileData object</param>
        /// <returns>AppFileData with Appfile nested object</returns>
        public Task<AppFileData?> GetByIdWithContentAsync(Guid id);

        /// <summary>
        /// Get total files count in storage
        /// </summary>
        /// <returns>Total files count</returns>
        public Task<int> GetFilesCountAsync();

        /// <summary>
        /// Get AppFileData object by id, with related Owner (User object),
        /// list of fileViwers (who also have permission to read this file), link for shortNavigation
        /// NO FILE CONTENT
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AppFileData object with related</returns>
        public Task<AppFileData?> GetByIdWithRelatedAsync(Guid id);

        /// <summary>
        /// Check, is this User owning this file
        /// TRUE if owning, FALSE otherwise
        /// </summary>
        /// <param name="fileId">File Id</param>
        /// <param name="OwnerId">User Id</param>
        /// <returns>TRUE if owning, FALSE otherwise</returns>
        public Task<bool> IsOwner(Guid fileId, Guid OwnerId);

        /// <summary>
        /// Return collection of AppFileData object shared with user
        /// without related data
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection of AppFileData object shared with user</returns>
        public IQueryable<AppFileData> GetShared(Guid userId);
    }
}
