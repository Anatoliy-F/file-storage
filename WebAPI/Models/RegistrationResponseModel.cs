namespace WebAPI.Models
{
    /// <summary>
    /// Represent data for sign up response
    /// </summary>
    public class RegistrationResponseModel
    {
        /// <summary>
        /// TRUE if registration successful, FALSE otherwise
        /// </summary>
        
        public bool Success { get; set; }
        
        /// <summary>
        /// Errors
        /// </summary>
        public IEnumerable<string>? Errors { get; set; }
    }
}
