namespace DataAccessLayer.Entities
{
    /// <summary>
    /// Entity to store file metadata
    /// </summary>
    public class AppFileData : BaseEntity
    {
        /// <summary>
        /// Timestamp for concurrency check
        /// </summary>
        public byte[]? TimeStamp { get; set; }

        /// <summary>
        /// File name. This name can broke UI, need sanitizing
        /// </summary>
        public string UntrustedName { get; set; } = "Empty name";

        /// <summary>
        /// Note for file from user
        /// </summary>
        public string Note { get; set; } = "Empty note";

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Upload dta and time
        /// </summary>
        public DateTime UploadDT { get; set; }

        /// <summary>
        /// If set in TRUE - file can be shared and accessed via short link
        /// </summary>
        public bool IsPublic { get; set; } = false;

        /// <summary>
        /// Id of file owner
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Navigation property to entity with file content
        /// </summary>
        public AppFile? AppFileNav { get; set; }

        /// <summary>
        /// Navigation property to file owner
        /// </summary>
        public AppUser? OwnerNav { get; set; }

        /// <summary>
        /// Navigation propery to Short Link (if exists)
        /// </summary>
        public ShortLink? ShortLinkNav { get; set; }

        /// <summary>
        /// Collection of users who have access to view the file
        /// </summary>
        public ICollection<AppUser>? FileViewers { get; set; }
    }
}
