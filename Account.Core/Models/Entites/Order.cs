using Account.Core.Enums;
using Account.Core.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Core.Models.Entites
{
    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } // Customer who placed the order

        public string? UserId { get; set; } // User (Admin) who created the order
        public AppUser User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Monetary properties
        public decimal TotalAmount { get; set; } = 0; // Calculated as the sum of OrderItems
        public decimal OutstandingBalance { get; set; } = 0;
        public decimal Discount { get; set; } = decimal.Zero;
        public DiscountType DiscountType { get; set; }

        public string Notes { get; set; } // Any additional order-specific notes

        // Navigation property for the order's items
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Calculated properties (Not mapped to the database)
        [NotMapped]
        public decimal TotalOrderItemsAmount => OrderItems.Sum(i => i.TotalPrice);

        [NotMapped]
        public decimal FinalAmount =>
            DiscountType == DiscountType.Percentage ? TotalAmount - (TotalAmount * Discount / 100) :
            DiscountType == DiscountType.FixedAmount ? TotalAmount - Discount : TotalAmount;

        // Method to calculate the order total based on its items
        public void CalculateTotalAmount()
        {
            TotalAmount = OrderItems.Sum(item => item.TotalPrice);
        }
    }
}
