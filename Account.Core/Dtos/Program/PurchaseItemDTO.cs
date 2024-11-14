using System.ComponentModel.DataAnnotations;

namespace Account.Core.Dtos.Program
{
    public class PurchaseItemDTO
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price per unit is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per unit must be greater than 0.")]
        public decimal PricePerUnit { get; set; }

        // Calculate total price in the service layer, or do it on the DTO if preferred.
        [Display(Name = "Total Price")]
        public decimal TotalPrice => Quantity * PricePerUnit;

        #region MyRegion
        //[Required(ErrorMessage = "Purchase ID is required.")]
        //public int PurchaseId { get; set; }

        //[Required(ErrorMessage = "Product name is required.")]
        //[StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        //public string ProductName { get; set; }

        //[Required(ErrorMessage = "Quantity is required.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        //public int Quantity { get; set; }

        //[Required(ErrorMessage = "Price per unit is required.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Price per unit must be greater than 0.")]
        //public decimal PricePerUnit { get; set; }

        //[Display(Name = "Total Price")]
        //public decimal TotalPrice { get; set; } 
        #endregion
    }
}