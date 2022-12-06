namespace WebAPI.Models
{
    /// <summary>
    /// Represent data for sign up response
    /// </summary>
    public class RegistrationResponseModel
    {
        public bool Success { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
