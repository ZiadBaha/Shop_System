using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSystem.Core.Models.Entites
{
    public class PurchaseItem : BaseEntity
    {
        public int PurchaseId { get; set; } 
        public Purchase Purchase { get; set; }

        public string ProductName { get; set; } 

        public int Quantity { get; set; } 

        public decimal PricePerUnit { get; set; } 

        public decimal TotalPrice { get; set; }
    }
}