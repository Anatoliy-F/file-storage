using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("FileData", Schema = "dbo")]
    public class AppFileData : BaseEntity
    {

        [Required]
        public string UnstrustedName { get; set; } = "Empty name";

        [Required]
        public string Note { get; set; } = "Empty note";

        [Required]
        public long Size { get; set; }

        [Required]
        public DateTime UploadDT { get; set; }

        [Required]
        public bool IsShared { get; set; } = false;

        [Required]
        public bool IsPublic { get; set; } = false;

        //TODO: make one-to-one in FluentAPI
        public int AppFileId { get; set; }

        public int AppUserId { get; set; }

        [ForeignKey(nameof(AppFileId))]
        public AppFile? AppFileNav { get; set; }

        [ForeignKey(nameof(AppUserId))]
        public AppUser? AppUserNav { get; set; }

        public ICollection<AppUser> FileViewers { get; set; } = new List<AppUser>();
    }
}
