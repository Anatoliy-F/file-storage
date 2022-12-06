using BuisnessLogicLayer.Models;

namespace BuisnessLogicLayer.Interfaces
{
    /// <summary>
    /// Provides operations with AppUser objects
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Checks if the user with the given email address is registered in the application
        /// </summary>
        /// <param name="userEmail">User email address</param>
        /// <returns>TRUE if user exist, FALSE otherwise</returns>
        public Task<ServiceResponse<bool>> IsExistByEmailAsync(string userEmail);

        /// <summary>
        /// Get user DTO by email
        /// </summary>
        /// <param name="userEmail">User email address</param>
        /// <returns>UserModel instanse</returns>
        public Task<ServiceResponse<UserModel>> GetByEmailAsync(string userEmail);

        /// <summary>
        /// Returns PaginationResultModel (properties for pagination on client side)
        /// with UserModel collections
        /// Used by ADMINISTRATORS
        /// </summary>
        /// <param name="query">QueryModel (Pagination, sort, filter info)</param>
        /// <returns>PaginationResultModel<UserModel></returns>
        public Task<ServiceResponse<PaginationResultModel<UserModel>>> GetAllAsync(QueryModel query);

        /// <summary>
        /// Return userModel by user Id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>UserModel instanse</returns>
        public Task<ServiceResponse<UserModel>> GetByIdAsync(Guid id);

        /// <summary>
        /// Delete user from application
        ///User's wuth Role "Administrator" don't deleted by this action
        /// </summary>
        /// <param name="userModel">UserModel instanse</param>
        /// <returns>TRUE is user deleted, FALSE otherwise</returns>
        public Task<ServiceResponse<bool>> DeleteAsync(UserModel userModel);

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="userModel">UserModel instanse</param>
        /// <returns>TRUE if updated successfully, FALSE otherwise</returns>
        public Task<ServiceResponse<bool>> UpdateAsync(UserModel userModel);
    }
}
