using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Models
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; } = false;
        
        public T? Data { get; set; } = default(T?);
        
        public string ErrorMessage { get; set; } = String.Empty;
    }
}
