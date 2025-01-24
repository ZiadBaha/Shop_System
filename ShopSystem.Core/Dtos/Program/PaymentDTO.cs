using System.ComponentModel.DataAnnotations;

namespace ShopSystem.Core.Dtos.Program
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Payment date is required.")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Amount is required.")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Information cannot exceed 500 characters.")]
        public string Info { get; set; }
    }
}