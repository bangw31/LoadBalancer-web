namespace ECommerceSystem.Shared.Entities
{
    public class UserLocation
    {
        public int UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public string Context { get; set; }
    }
}