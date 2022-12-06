namespace WebAPI.Models
{
    /// <summary>
    /// Using to bind Note to File object
    /// cause file read from stream model binding doesn't read the form
    /// </summary>
    public class FormData
    {
        /// <summary>
        /// Comment to file
        /// </summary>
        public string Note { get; set; } = String.Empty;
    }
}
