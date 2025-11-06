using ECommerceSystem.GUI.Apis;
using ECommerceSystem.Shared.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Controllers
{
    //[Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderApi _orderApi;

        public OrderController(IOrderApi orderApi)
        {
            _orderApi = orderApi;
        }

        public IActionResult Create()
        {
            return View(new CreateOrderRequest { Items = new List<OrderItemDTO>() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _orderApi.CreateOrderAsync(model);
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index", "Home"); // Hoặc trang xác nhận đơn hàng
                ModelState.AddModelError("", "Lỗi khi tạo đơn hàng.");
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Lỗi khi tạo đơn hàng.");
                return View(model);
            }
        }
    }
}