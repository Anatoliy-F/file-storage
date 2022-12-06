namespace BuisnessLogicLayer.Models
{
    /// <summary>
    /// Readonly DTO representing file, without any related information
    /// </summary>
    public class ShortFileDataModel
    {
        /// <summary>
        /// File metadata object Id
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;

        // <summary>
        /// File name, untrusted? should be sanitized before using in UI
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Comment to file
        /// </summary>
        public string Note { get; set; } = string.Empty;

        // <summary>
        /// File size in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Upload DateTime
        /// </summary>
        public DateTime UploadDateTime { get; set; }
    }
}
