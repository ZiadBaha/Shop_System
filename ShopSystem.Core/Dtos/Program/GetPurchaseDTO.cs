using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class GetPurchaseDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Notes { get; set; }
        public List<PurchaseItemDTO> PurchaseItems { get; set; }
        public MerchantDTO Merchant { get; set; }
    }
}
