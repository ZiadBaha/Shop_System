using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class CustomerDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Money owed cannot be negative.")]
        public decimal? MoneyOwed { get; set; }

      

        //[Display(Name = "Total Payments")]
        //public decimal TotalPayments { get; set; }

        //[Display(Name = "Outstanding Balance")]
        //public decimal OutstandingBalance { get; set; }

        //[Display(Name = "Total Order Amount")]
        //public decimal TotalOrderAmount { get; set; }
    }
}
