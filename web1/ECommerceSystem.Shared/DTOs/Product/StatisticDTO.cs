namespace ECommerceSystem.Shared.DTOs.Product
{
    public class StatisticDTO
    {
        public List<object> Revenue { get; set; } // { Date, Value }
        public Dictionary<string, int> OrderCount { get; set; } // Thay List<int> thành Dictionary<string, int>
        public List<object> TopProducts { get; set; } // Thêm property mới
        public List<object> LowStock { get; set; } // Đã có từ trước
    }
    public class RevenueData
    {
        public string Date { get; set; }
        public decimal Value { get; set; }
    }

    public class TopProductData
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
    public class UserActivityDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ActivityType { get; set; }
        public int Count { get; set; }
        public DateTime Time { get; set; }
    }
}