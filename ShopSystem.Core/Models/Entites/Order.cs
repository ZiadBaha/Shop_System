using ShopSystem.Core.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSystem.Core.Models.Entites
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; } 
        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } 
        public decimal TotalDiscount { get; set; } 
        public decimal TotalAmount { get; set; }
        public Customer Customer { get; set; } 
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string? Notes { get; set; }

    }
}