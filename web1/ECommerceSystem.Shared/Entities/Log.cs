namespace ECommerceSystem.Shared.Entities
{
    public class Log
    {
        public string RequestId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Endpoint { get; set; }
        public string UserAgent { get; set; }
        public int Status { get; set; }
    }
}