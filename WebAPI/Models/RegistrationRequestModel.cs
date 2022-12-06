using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    /// <summary>
    /// Represent data for sign up request
    /// </summary>
    public class RegistrationRequestModel : IValidatableObject
    {
        /// <summary>
        /// User name
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        /// <summary>
        /// Repeat of user password
        /// </summary>
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        /// <summary>
        /// Implementation of IValidatableObject
        /// </summary>
        /// <param name="validationContext"><see cref="ValidationContext"/></param>
        /// <returns>Collection of validation errors</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new();

            if (string.IsNullOrWhiteSpace(Email))
                result.Add(new ValidationResult("Email is empty"));
            if (string.IsNullOrWhiteSpace(Password))
                result.Add(new ValidationResult("Password is empty"));
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                result.Add(new ValidationResult("ConfirmPassword is empty"));
            if (Password != ConfirmPassword)
                result.Add(new ValidationResult("Password and ConfirmPasword not equal"));

            return result;
        }
    }
}
