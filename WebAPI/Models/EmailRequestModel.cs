using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class EmailRequestModel
    {
        [Required(ErrorMessage = "Address is required.")]
        [EmailAddress]
        public string Address { get; set; } = String.Empty;
    }
}
