using Account.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Core.Models.Entites
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public bool IsStock { get; set; }

        
        
        // Purchase and selling prices 
        public decimal PurchasePrice { get; set; }  // The price at which the product is bought
        public decimal SellingPrice { get; set; }   // The price at which the product is sold

        // Property for profit, calculated based on PurchasePrice and SellingPrice
        [NotMapped]
        public decimal Profit => SellingPrice - PurchasePrice;
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Many-to-Many relationship through OrderItem
        public ProductStatus? Status { get; set; } = ProductStatus.Active;

        public ICollection<OrderItem> OrderItems { get; set; }

        // Total inventory value for this product
        [NotMapped]
        public decimal TotalValue => Quantity.GetValueOrDefault() * PurchasePrice;

    }
}