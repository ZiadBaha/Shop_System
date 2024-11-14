using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Core.Models.Entites
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } // The order to which this item belongs

        public int ProductId { get; set; }
        public Product Product { get; set; } // Product associated with this order item

        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }

        // Calculated property for the total price of this order item
        [NotMapped]
        public decimal TotalPrice => Quantity * PricePerUnit;

    }
}