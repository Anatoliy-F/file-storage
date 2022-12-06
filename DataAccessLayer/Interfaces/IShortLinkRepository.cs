using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    /// <summary>
    /// Describe operations with ShortLink objects
    /// </summary>
    public interface IShortLinkRepository : IRepository<ShortLink>
    {
        /// <summary>
        /// Check is this link already exists,
        /// if exist return TRUE, FALSE otherwise
        /// </summary>
        /// <param name="link">string link</param>
        /// <returns>If link exist return TRUE, FALSE otherwise</returns>
        public Task<bool> IsCollision(string link);

        /// <summary>
        /// Check if link already exists for this file
        /// if exist return TRUE, FALSE otherwise
        /// </summary>
        /// <param name="fileId">AppFileData Id</param>
        /// <returns>If link for this AppFileData exist return TRUE, FALSE otherwise</returns>
        public Task<bool> IsExist(Guid fileId);

        /// <summary>
        /// Return AppFileData object with content (AppFile object)
        /// </summary>
        /// <param name="link">string link</param>
        /// <returns>file metadata with content</returns>
        public Task<AppFileData?> GetFileContentByLinkAsync(string link);

        /// <summary>
        /// Return ShortLink object by link
        /// </summary>
        /// <param name="link">string link</param>
        /// <returns>ShortLink</returns>
        public Task<ShortLink?> GetShortLinkAsync(string link);

        // <summary>
        /// Return AppFileData object without content
        /// for preview purpose
        /// </summary>
        /// <param name="link">string link</param>
        /// <returns>file metadata without content</returns>
        
        public Task<AppFileData?> GetFileDataByLinkAsync(string link);

        /// <summary>
        /// Return AppFileData object with related information
        /// for preview purpose
        /// </summary>
        /// <param name="link">string link</param>
        /// <returns>file metadata with related objects</returns>
        public Task<ShortLink?> GetShortLinkWithRelatedAsync(string link);
    }
}
