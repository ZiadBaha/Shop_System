namespace ShopSystem.Core.Dtos.Program
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string UserId { get; set; }
        public string? Notes { get; set; }
        public decimal TotalDiscount { get; set; } 
        public decimal TotalAmount { get; set; } 
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}