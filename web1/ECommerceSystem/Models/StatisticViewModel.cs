using ECommerceSystem.Shared.DTOs;

namespace ECommerceSystem.Models
{
    public class StatisticViewModel
    {
        public List<RevenueData> Revenue { get; set; }
        public Dictionary<string, int> OrderCount { get; set; }
        public List<TopProductData> TopProducts { get; set; }
        public List<ProductViewModel> LowStock { get; set; }
        public List<UserActivityViewModel> Activities { get; set; }
    }

   
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }

    public class UserActivityViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ActivityType { get; set; } // Đổi từ Action để tránh xung đột
        public int Count { get; set; }
        public DateTime Time { get; set; }
    }
}