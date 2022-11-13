using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class QueryOptionsModel
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string? sortColumn { get; set; }
        public string? sortOrder { get; set; }
        public string? filterColumn { get; set; }
        public string? filterQuery { get; set; }
    }
}
