using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ECommerceSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync("api/suggestions");
                if (response.IsSuccessStatusCode)
                {
                    var suggestions = await response.Content.ReadFromJsonAsync<dynamic>();
                    ViewBag.Suggestions = suggestions;
                }
                else
                {
                    ViewBag.Error = "Failed to load suggestions.";
                }
            }
            catch
            {
                ViewBag.Error = "Error connecting to API.";
            }
            return View();
        }
    }
}