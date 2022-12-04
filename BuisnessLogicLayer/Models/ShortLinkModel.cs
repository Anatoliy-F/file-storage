using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer.Models
{
    public class ShortLinkModel
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Link { get; set; } = String.Empty;
        public Guid FileId { get; set; } = Guid.Empty;
    }
}
