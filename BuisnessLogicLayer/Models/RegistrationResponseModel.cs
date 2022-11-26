namespace BuisnessLogicLayer.Models
{
    public class RegistrationResponseModel
    {
        public bool Success { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
