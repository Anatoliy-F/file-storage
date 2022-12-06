using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BuisnessLogicLayer.Models
{
    /// <summary>
    /// DTO representing file
    /// </summary>
    public class FileDataModel : IValidatableObject
    {
        /// <summary>
        /// File metadata object Id
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Concurrency check token
        /// </summary>
        public byte[]? TimeStamp { get; set; }

        /// <summary>
        /// File content
        /// </summary>
        public byte[]? Content { get; set; }

        /// <summary>
        /// File name, untrusted? should be sanitized before using in UI
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Comment to file
        /// </summary>
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Upload DateTime
        /// </summary>
        public DateTime UploadDateTime { get; set; }

        /// <summary>
        /// if TRUE it is possible to grant access to the file 
        /// and create a short link to the file
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Owner Id (AppUser object)
        /// </summary>
        public Guid OwnerId { get; set; } = Guid.Empty;

        /// <summary>
        /// Owner name
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// Presents a short link for quick access
        /// </summary>
        public string ShortLink { get; set; } = string.Empty;

        /// <summary>
        /// Collection of users who have been granted access to the file
        /// </summary>
        public ICollection<UserModel> Viewers { get; set; } = new List<UserModel>();

        /// <summary>
        /// Validate object by business rules
        /// </summary>
        /// <param name="validationContext">Describes context in which validation performed</param>
        /// <returns>Collection of validation results</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Name))
                result.Add(new ValidationResult("File name is empty"));
            if (string.IsNullOrWhiteSpace(Note))
                result.Add(new ValidationResult("File note is empty"));
            if (Size <= 0)
                result.Add(new ValidationResult("File size less or equal 0"));
            if(!IsPublic && !string.IsNullOrEmpty(ShortLink) ||
                !IsPublic && Viewers.Count > 0)
            {
                result.Add(new ValidationResult("Access level inconsistency"));
            }
            
            return result;
        }
    }
}
