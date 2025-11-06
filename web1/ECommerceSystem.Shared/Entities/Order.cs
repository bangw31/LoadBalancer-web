namespace ECommerceSystem.Shared.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string DeliveryLocation { get; set; }
        public string? QrCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<OrderItem> OrderItems { get; set; }

    }
}