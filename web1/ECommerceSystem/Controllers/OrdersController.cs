using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> History()
        {
            var client = _httpClientFactory.CreateClient();
            var orders = await client.GetFromJsonAsync<dynamic>("https://localhost:7068/api/orders");
            return View(orders);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}