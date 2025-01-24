using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class MerchantDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Merchant name is required.")]
        [StringLength(100, ErrorMessage = "Merchant name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Outstanding balance cannot be negative.")]
        public decimal? OutstandingBalance { get; set; }

        public List<PurchaseDTO> Purchases { get; set; } = new List<PurchaseDTO>();

        [Display(Name = "Total Purchase Amount")]
        public decimal TotalPurchaseAmount { get; set; }

        [Display(Name = "Total Outstanding Balance")]
        public decimal TotalOutstandingBalance { get; set; }
    }
}
