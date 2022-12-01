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

        /// <summary>
        /// Check is user exist in database
        /// use NORMALIZED EMAIL (in UPPERCASE)
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns>TRUE if user exist? FALSE otherwise</returns>
        public Task<bool> IsExistByEmailAsync(string userEmail);

        /// <summary>
        /// Return user by Email or NULL
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public Task<AppUser?> GetByEmailAsync(string userEmail);
    }
}
