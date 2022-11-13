using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("File", Schema = "dbo")]
    public class AppFile : BaseEntity
    {
        [Required]
        public byte[] Content { get; set; } = Array.Empty<byte>();

        //public int AppFileDataId { get; set; }

        //[ForeignKey(nameof(AppFileDataId))]
        public AppFileData? AppFileDataNav { get; set; }
    }
}
