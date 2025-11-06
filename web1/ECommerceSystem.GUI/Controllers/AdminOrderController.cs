using ECommerceSystem.GUI.Apis;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderApi _orderAdminApi;

        public AdminOrderController(IOrderApi orderAdminApi)
        {
            _orderAdminApi = orderAdminApi;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderAdminApi.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderAdminApi.GetOrderByIdAsync(id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var result = await _orderAdminApi.UpdateOrderStatusAsync(id, status);
            return RedirectToAction("Index");
        }
    }
}
