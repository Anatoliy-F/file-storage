using BuisnessLogicLayer.Enums;

namespace BuisnessLogicLayer.Models
{
    public class ServiceResponse<T>
    {
        public ResponseResult ResponseResult { get; set; } = ResponseResult.Error;
        
        public T? Data { get; set; } = default(T?);
        
        public string ErrorMessage { get; set; } = String.Empty;
    }
}
