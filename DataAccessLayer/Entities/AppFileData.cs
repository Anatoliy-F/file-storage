namespace DataAccessLayer.Entities
{
    public class AppFileData : BaseEntity
    {
        public string UnstrustedName { get; set; } = "Empty name";

        public string Note { get; set; } = "Empty note";

        public long Size { get; set; }

        public DateTime UploadDT { get; set; }

        public bool IsPublic { get; set; } = false;

        public Guid AppFileId { get; set; }

        public Guid OwnerId { get; set; }

        public AppFile? AppFileNav { get; set; }

        public AppUser? OwnerNav { get; set; }

        public ShortLink? ShortLinkNav { get; set; }

        public ICollection<AppUser>? FileViewers { get; set; }
    }
}
