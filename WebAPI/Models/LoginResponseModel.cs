namespace WebAPI.Models
{
    /// <summary>
    /// Represent data for sign in response
    /// </summary>
    public class LoginResponseModel
    {
        /// <summary>
        /// TRUE if login successful, FALSE otherwise
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Describe result of login request
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// TRUE if user has role "Administrator"
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// JWT Bearer
        /// </summary>
        public string? Token { get; set; }
    }
}
