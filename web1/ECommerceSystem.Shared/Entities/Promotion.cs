namespace ECommerceSystem.Shared.Entities
{
    public class Promotion
    {
        public string Id { get; set; }
        public decimal Discount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}