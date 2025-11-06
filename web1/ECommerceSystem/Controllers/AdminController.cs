//using ECommerceSystem.Models;
//using ECommerceSystem.Shared.DTOs;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace ECommerceSystem.Controllers
//{
//    [Authorize(Roles = "Admin")]
//    public class AdminController : Controller
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
        
//        public AdminController(IHttpClientFactory httpClientFactory)
//        {
//            _httpClientFactory = httpClientFactory;
//        }

//        public async Task<IActionResult> Dashboard()
//        {
//            var model = new StatisticViewModel
//            {
//                Revenue = new List<RevenueData>(),
//                OrderCount = new Dictionary<string, int>(),
//                TopProducts = new List<TopProductData>(),
//                LowStock = new List<ProductViewModel>(),
//                Activities = new List<UserActivityViewModel>()
//            };

//            try
//            {
//                var client = _httpClientFactory.CreateClient("ApiClient");

//                // Lấy thống kê
//                var statsResponse = await client.GetAsync("api/admin/statistics?type=revenue&period=month");
//                if (!statsResponse.IsSuccessStatusCode)
//                {
//                    ViewBag.Error = "Failed to load statistics.";
//                    return View(model);
//                }
//                var stats = await statsResponse.Content.ReadFromJsonAsync<StatisticDTO>();
//                model.Revenue = stats.Revenue?.Select(r => new RevenueData
//                {
//                    Date = r.Date,
//                    Value = r.Value
//                }).ToList() ?? new List<RevenueData>();
//                model.OrderCount = stats.OrderCount ?? new Dictionary<string, int>();
//                model.TopProducts = stats.TopProducts?.Select(p => new TopProductData
//                {
//                    Name = p.Name,
//                    Quantity = p.Quantity
//                }).ToList() ?? new List<TopProductData>();

//                // Lấy danh sách sản phẩm tồn kho thấp
//                var inventoryResponse = await client.GetAsync("api/admin/inventory");
//                if (!inventoryResponse.IsSuccessStatusCode)
//                {
//                    ViewBag.Error = "Failed to load inventory.";
//                    return View(model);
//                }
//                var inventory = await inventoryResponse.Content.ReadFromJsonAsync<List<ProductDTO>>();
//                model.LowStock = inventory.Select(p => new ProductViewModel
//                {
//                    Id = p.Id,
//                    Name = p.Name,
//                    Stock = p.Stock,
//                    Price = p.Price
//                }).ToList();

//                // Lấy hoạt động người dùng
//                var activitiesResponse = await client.GetAsync("api/admin/user-activity");
//                if (!activitiesResponse.IsSuccessStatusCode)
//                {
//                    ViewBag.Error = "Failed to load user activities.";
//                    return View(model);
//                }
//                var activities = await activitiesResponse.Content.ReadFromJsonAsync<List<UserActivityDTO>>();
//                model.Activities = activities.Select(a => new UserActivityViewModel
//                {
//                    Id = a.Id,
//                    UserName = a.UserName,
//                    ActivityType = a.ActivityType,
//                    Count = a.Count,
//                    Time = a.Time
//                }).ToList();

//                return View(model);
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Error = $"Error connecting to API: {ex.Message}";
//                return View(model);
//            }
//        }
//    }
//}