using Account.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Dtos.Program
{
    public class CreateOrderDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative.")]
        public decimal? TotalAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Outstanding balance cannot be negative.")]
        public decimal? OutstandingBalance { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount cannot be negative.")]
        public decimal? Discount { get; set; }

        public DiscountType DiscountType { get; set; }

        public List<CreateOrderItemDTO> OrderItems { get; set; }/* = new List<CreateOrderItemDTO>();*/
    }
}
