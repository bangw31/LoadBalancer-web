using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var products = await client.GetFromJsonAsync<dynamic>("https://localhost:7068/api/products");
            return View(products);
        }

        public IActionResult Detail(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }
    }
}