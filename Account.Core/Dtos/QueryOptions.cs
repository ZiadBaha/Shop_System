using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Dtos
{
    public class QueryOptions
    {
        public string? Search { get; set; }
        public string SortField { get; set; } = "Date"; // Default sort field
        public bool SortDescending { get; set; } = false;
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
}
