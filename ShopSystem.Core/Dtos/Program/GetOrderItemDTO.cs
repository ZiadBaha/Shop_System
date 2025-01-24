using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class GetOrderItemDTO
    {

        public int ProductId { get; set; }
        public string? ProductName { get; set; } 
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }
    }
}
