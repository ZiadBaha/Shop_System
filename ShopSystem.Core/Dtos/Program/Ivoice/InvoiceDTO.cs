using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Dtos.Program.Ivoice
{
    public class InvoiceDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }

        public string UserName { get; set; }
        public List<InvoiceItemDTO> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Notes { get; set; } // Optional notes
    }
}
