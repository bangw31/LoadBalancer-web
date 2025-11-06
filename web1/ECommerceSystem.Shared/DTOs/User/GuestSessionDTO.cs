namespace ECommerceSystem.Shared.DTOs.User
{
    public class GuestSessionDTO
    {
        public string SessionId { get; set; }
        public string Ip { get; set; }
        public List<int> ViewedProducts { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

