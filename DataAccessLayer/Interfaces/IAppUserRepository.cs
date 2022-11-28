using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public interface IAppUserRepository : IRepository<AppUser>
    {
        /// <summary>
        /// Get AppUser object by id, with related AppFiles - collection of AppFileData (owned),
        /// ReadOnlyFiles - collection of AppFileData with read-only access (not owned), with info about owner
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>AppUser object with related data</returns>
        public Task<AppUser?> GetByIdWithRelatedAsync(Guid userId);

        /// <summary>
        /// Returns the number of files the user does not own but can view
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Number of read-only accessed files</returns>
        public Task<int> GetReadOnlyFilesCountAsync(Guid userId);
    }
}
