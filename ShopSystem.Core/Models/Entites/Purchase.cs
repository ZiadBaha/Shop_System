using System.ComponentModel.DataAnnotations.Schema;

namespace ShopSystem.Core.Models.Entites
{
    public class Purchase : BaseEntity
    {

        public int MerchantId { get; set; }
        public Merchant Merchant { get; set; } // Navigation property to Merchant

        public DateTime OrderDate { get; set; } = DateTime.Now; 

        public decimal? TotalAmount { get; set; }

        public string? Notes { get; set; } 

        public ICollection<PurchaseItem> PurchaseItems { get; set; }
        public Purchase()
        {
            PurchaseItems = new List<PurchaseItem>();
        }

    }
}