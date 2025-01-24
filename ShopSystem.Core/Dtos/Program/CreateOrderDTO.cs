using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class CreateOrderDTO
    {
        public int CustomerId { get; set; }  
        public List<OrderItemDTO> OrderItems { get; set; }  
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? Notes { get; set; }
    }
}
