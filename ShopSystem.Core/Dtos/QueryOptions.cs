using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos
{
    public class QueryOptions
    {
        public string? Search { get; set; }
        public string SortField { get; set; } = "Date"; // Default sort field
        public bool SortDescending { get; set; } = false;
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? Name { get; set; } 

        public DateTime? StartDate { get; set; } // New field for start date filter
        public DateTime? EndDate { get; set; }   // New field for end date filter
    }
}
