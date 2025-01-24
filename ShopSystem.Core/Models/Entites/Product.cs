using ShopSystem.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopSystem.Core.Models.Entites
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public bool IsStock { get; set; } = true;
        [Required]
        public string UniqueNumber { get; set; }
        public decimal PurchasePrice { get; set; }  
        public decimal SellingPrice { get; set; }   
        [NotMapped]
        public decimal Profit => SellingPrice - PurchasePrice;
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ProductStatus? Status { get; set; } = ProductStatus.Active;
        public ICollection<OrderItem> OrderItems { get; set; }

        [NotMapped]
        public decimal TotalValue => Quantity.GetValueOrDefault() * PurchasePrice;

    }
}