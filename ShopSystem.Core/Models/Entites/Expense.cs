using ShopSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Models.Entites
{
    public class Expense : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public ExpenseCategory? Category { get; set; }
        public string Info { get; set; }
    }
}
