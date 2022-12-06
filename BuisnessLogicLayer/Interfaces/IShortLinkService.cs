using BuisnessLogicLayer.Models;

namespace BuisnessLogicLayer.Interfaces
{
    /// <summary>
    /// Provides operations with ShortLink objects
    /// </summary>
    public interface IShortLinkService
    {
        /// <summary>
        /// Generate ShortLink for existing file, checking file existanse and its access level
        /// </summary>
        /// <param name="fileId">File id</param>
        /// <returns>ShortLinkModel instanse</returns>
        public Task<ServiceResponse<ShortLinkModel>> GenerateForFileByIdAsync(Guid fileId);

        /// <summary>
        /// Return FileDataModel with content by short link, 
        /// for downloading purposes
        /// </summary>
        /// <param name="link">short link</param>
        /// <returns>FileDataModel with content</returns>
        public Task<ServiceResponse<FileDataModel>> GetFileByShortLinkAsync(string link);

        /// <summary>
        /// Delete link for file
        /// </summary>
        /// <param name="link">short link</param>
        /// <returns>TRUE if link succesfully deleting</returns>
        public Task<ServiceResponse<bool>> DeleteLinkAsync(string link);

        /// <summary>
        /// Return ShortFileDataModel to preview
        /// </summary>
        /// <param name="link">short link</param>
        /// <returns>ShortFileDataModel instanse</returns>
        public Task<ServiceResponse<ShortFileDataModel>> GetShortFileDataAsync(string link);
    }
}
