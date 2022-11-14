using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class ShortLink : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Link { get; set; } = string.Empty;
        
        public Guid AppFileDataId { get; set; }

        [ForeignKey(nameof(AppFileDataId))]
        public AppFileData? AppFileDataNav { get; set; }
    }
}
