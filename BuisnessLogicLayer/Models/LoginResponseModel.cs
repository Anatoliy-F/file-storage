using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BuisnessLogicLayer.Models
{
    public class LoginResponseModel
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public string? Token { get; set; }
    }
}
