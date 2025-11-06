namespace ECommerceSystem.Shared.Entities
{
    public class UserPreference
    {
        public int? UserId { get; set; }
        public string GuestId { get; set; }
        public List<string> SearchHistory { get; set; }
        public List<int> RecommendedProducts { get; set; }
        public DateTime Timestamp { get; set; }
    }
}