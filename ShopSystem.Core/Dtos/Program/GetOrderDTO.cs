using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class GetOrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalAmount { get; set; } // Calculated field: TotalAmount - TotalDiscount
        public GetCustomerDTO? Customer { get; set; } // Customer details
        public string? UserId { get; set; }
        public UserDto? User { get; set; } // User (Admin) details
        public List<GetOrderItemDTO>? OrderItems { get; set; } // List of items in the order
    }
}
