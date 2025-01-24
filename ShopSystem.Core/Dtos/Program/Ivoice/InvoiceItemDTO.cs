namespace ShopSystem.Core.Dtos.Program.Ivoice
{
    public class InvoiceItemDTO
    {
        public string ProductName { get; set; }
        public string ProductNameArabic { get; set; } // Arabic version of the product name

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
    }
}