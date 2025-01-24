using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSystem.Core.Models.Entites
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public ICollection<Order> Orders { get; set; }
        public decimal? MoneyOwed { get; set; } = 0;
        public ICollection<Payment> Payments { get; set; }

        // Calculated properties with [NotMapped] attributes
        [NotMapped]
        public decimal TotalPayments => Payments != null ? Payments.Sum(p => p.Amount) : 0;

        [NotMapped]
        public decimal? OutstandingBalance => (Orders != null ? Orders.Sum(o => o.TotalAmount) : 0) - TotalPayments;

        [NotMapped]
        public decimal? TotalOrderAmount => Orders != null ? Orders.Sum(o => o.TotalAmount) : 0;
    }
}