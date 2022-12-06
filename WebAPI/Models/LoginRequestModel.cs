using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    /// <summary>
    /// Represent data for sign in request
    /// </summary>
    public class LoginRequestModel
    {
        /// <summary>
        /// Email address
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}
