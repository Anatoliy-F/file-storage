using BuisnessLogicLayer.Enums;

namespace BuisnessLogicLayer.Models
{
    /// <summary>
    /// The object returned by the service method
    /// </summary>
    /// <typeparam name="T">Represent result Data type</typeparam>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Represents the result of a request to the service
        /// </summary>
        public ResponseResult ResponseResult { get; set; } = ResponseResult.Error;
        
        /// <summary>
        /// Returned data
        /// </summary>
        public T? Data { get; set; } = default(T?);
        
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; } = String.Empty;
    }
}
