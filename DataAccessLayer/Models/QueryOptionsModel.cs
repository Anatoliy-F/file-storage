﻿using System;
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
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; } = null;
        public string? SortOrder { get; set; } = null;
        public string? FilterColumn { get; set; } = null;
        public string? FilterQuery { get; set; } = null;
    }
}
