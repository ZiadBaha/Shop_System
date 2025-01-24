using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Models.Entites
{
    public class Merchant : BaseEntity
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public decimal? OutstandingBalance { get; set; } = 0;


        // Total amount spent in purchases
        [NotMapped]
        public decimal? TotalPurchaseAmount => Purchases.Sum(p => p.TotalAmount);

        // Outstanding balance owed to the merchant
        [NotMapped]
        public decimal? TotalOutstandingBalance => Purchases.Sum(p => p.TotalAmount);
    }
}
