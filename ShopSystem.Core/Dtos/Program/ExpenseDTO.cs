using ShopSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class ExpenseDTO
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; } = DateTime.Now;

        public ExpenseCategory? Category { get; set; }

        [StringLength(500, ErrorMessage = "Information cannot exceed 500 characters.")]
        public string Info { get; set; }
    }
}
