using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Models.Entites
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

    #region MyRegion
    //// Total of all payments made by the customer
    //[NotMapped]
    //public decimal TotalPayments => Payments.Sum(p => p.Amount);

    //// Outstanding balance owed by the customer after payments
    //[NotMapped]
    //public decimal? OutstandingBalance => Orders.Sum(o => o.TotalAmount) - TotalPayments;

    //// Total of all orders placed by the customer
    //[NotMapped]
    //public decimal? TotalOrderAmount => Orders.Sum(o => o.TotalAmount); 
    #endregion
}

