using System.ComponentModel.DataAnnotations;

namespace ShopSystem.Core.Dtos.Program
{
    public class PurchaseItemDTO
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }
}