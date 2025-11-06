namespace ECommerceSystem.Shared.DTOs.Product
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string DeliveryLocation { get; set; }
        public string? QrCode { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }
}

