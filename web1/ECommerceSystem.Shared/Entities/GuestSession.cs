namespace ECommerceSystem.Shared.Entities
{
    public class GuestSession
    {
        public string SessionId { get; set; }
        public string Ip { get; set; }
        public List<int> ViewedProducts { get; set; }
        public DateTime Timestamp { get; set; }
    }
}