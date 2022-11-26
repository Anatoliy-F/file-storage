using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BuisnessLogicLayer.Models
{
    public class FileDataModel : IValidatableObject
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public long Size { get; set; }

        public DateTime UploadDateTime { get; set; }
        
        public bool IsPublic { get; set; }

        public Guid OwnerId { get; set; } = Guid.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public string ShortLink { get; set; } = string.Empty;

        public ICollection<Guid> Viewers { get; set; } = new List<Guid>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Name))
                result.Add(new ValidationResult("File name is empty"));
            if (string.IsNullOrWhiteSpace(Note))
                result.Add(new ValidationResult("File note is empty"));
            if (Size <= 0)
                result.Add(new ValidationResult("File size less or equal 0"));

            return result;
        }
    }
}
