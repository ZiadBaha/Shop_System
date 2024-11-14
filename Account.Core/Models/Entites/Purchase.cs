using System.ComponentModel.DataAnnotations.Schema;

namespace Account.Core.Models.Entites
{
    public class Purchase : BaseEntity
    {
        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal? TotalAmount { get; set; } = 0;
        public decimal? OutstandingBalance { get; set; } = 0;
        public ICollection<PurchaseItem> Products { get; set; }
        public bool IsPaid { get; set; } 
        public decimal Amount { get; set; } 

        public string Notes { get; set; }

        // Total of all purchase items
        [NotMapped]
        public decimal TotalPurchaseItemsAmount => Products.Sum(i => i.TotalPrice);
    }
}