using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program
{
    public class GetPaymentDTO
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string CustomerName { get; set; } 

        public string Info { get; set; }
    }
}
