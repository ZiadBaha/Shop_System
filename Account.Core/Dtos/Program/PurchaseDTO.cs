using System.ComponentModel.DataAnnotations;

namespace Account.Core.Dtos.Program
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Merchant ID is required.")]
        public int MerchantId { get; set; }

        [Required(ErrorMessage = "Order date is required.")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative.")]
        public decimal? TotalAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Outstanding balance cannot be negative.")]
        public decimal? OutstandingBalance { get; set; }

        public List<PurchaseItemDTO> PurchaseItems { get; set; } = new List<PurchaseItemDTO>();

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string Notes { get; set; }

        public bool IsPaid { get; set; } 
        public decimal Amount { get; set; } 


    }
}