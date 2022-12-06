namespace WebAPI.Models
{
    /// <summary>
    /// Represent data for sign in response
    /// </summary>
    public class LoginResponseModel
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public bool IsAdmin { get; set; }

        public string? Token { get; set; }
    }
}
