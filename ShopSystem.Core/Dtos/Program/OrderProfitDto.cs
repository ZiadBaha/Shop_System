using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class OrderProfitDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Profit { get; set; }
    }
}
