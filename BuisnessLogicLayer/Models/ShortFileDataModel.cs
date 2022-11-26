using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Models
{
    public class ShortFileDataModel
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public long Size { get; set; }

        public DateTime UploadDateTime { get; set; }
    }
}
