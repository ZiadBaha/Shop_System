namespace ShopSystem.Core.Models.Entites
{
    public class Payment : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string Info { get; set; }
    }
}