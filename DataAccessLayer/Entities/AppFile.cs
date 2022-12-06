namespace DataAccessLayer.Entities
{
    /// <summary>
    /// Entity for storing File in Blob
    /// </summary>
    public class AppFile : BaseEntity
    {
        /// <summary>
        /// Byte array with file content
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Foreign key for related AppFileData entity
        /// </summary>
        public Guid AppFileDataId { get; set; }

        /// <summary>
        /// Navigation property for AppFileData entity
        /// </summary>
        public AppFileData? AppFileDataNav { get; set; }
    }
}
