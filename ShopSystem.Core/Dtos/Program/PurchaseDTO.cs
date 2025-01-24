using System.ComponentModel.DataAnnotations;

namespace ShopSystem.Core.Dtos.Program
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Notes { get; set; }
        public List<PurchaseItemDTO> PurchaseItems { get; set; }

    }
}