namespace BuisnessLogicLayer.Models
{
    /// <summary>
    /// DTO representing short link for quick access to file
    /// </summary>
    public class ShortLinkModel
    {
        /// <summary>
        /// Short link object Id
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Link in Base64UrlEncode format
        /// </summary>
        public string Link { get; set; } = String.Empty;

        /// <summary>
        /// The id of the file being accessed
        /// </summary>
        public Guid FileId { get; set; } = Guid.Empty;
    }
}
