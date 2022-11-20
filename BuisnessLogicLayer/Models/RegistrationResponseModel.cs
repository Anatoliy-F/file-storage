using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Models
{
    public class RegistrationResponseModel
    {
        public bool Success { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
