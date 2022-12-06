namespace BuisnessLogicLayer.Models
{
    /// <summary>
    /// DTO representing user without sensitive information
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// User Id
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Concurrency check token
        /// </summary>
        public string Concurrency { get; set; } = String.Empty;

        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = String.Empty;
    }
}
