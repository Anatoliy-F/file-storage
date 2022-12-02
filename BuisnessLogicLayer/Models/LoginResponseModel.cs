namespace BuisnessLogicLayer.Models
{
    public class LoginResponseModel
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public bool IsAdmin { get; set; }

        public string? Token { get; set; }
    }
}
