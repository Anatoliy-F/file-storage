namespace DataAccessLayer.Entities
{
    /// <summary>
    /// Entity for implementing access to files via a short link
    /// </summary>
    public class ShortLink : BaseEntity
    {
        /// <summary>
        /// Short Link text representation
        /// </summary>
        public string Link { get; set; } = string.Empty;
        
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
