using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    /// <summary>
    /// Represent data for sign in request
    /// </summary>
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}
