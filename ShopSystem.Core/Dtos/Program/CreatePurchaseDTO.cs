using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class CreatePurchaseDTO
    {
        public int MerchantId { get; set; }
        public string? Notes { get; set; }
        public DateTime OrderDate { get; set; }=DateTime.Now;
        public List<CreatePurchaseItemDTO> PurchaseItems { get; set; }
    }
}
