using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    [NotMapped]
    public class QueryOptionsModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string? sortColumn { get; set; } = null;
        public string? sortOrder { get; set; } = null;
        public string? filterColumn { get; set; } = null;
        public string? filterQuery { get; set; } = null;
    }
}
