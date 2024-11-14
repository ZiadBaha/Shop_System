using Account.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Account.Core.Dtos.Program
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? OutstandingBalance { get; set; }
        public decimal? Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
        public decimal FinalAmount { get; set; }
    }
}