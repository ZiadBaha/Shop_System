namespace ShopSystem.Core.Dtos.Program
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }  
        public int Quantity { get; set; }  
        public decimal Discount { get; set; }  
        public decimal SubTotal { get; set; } 
    }
}