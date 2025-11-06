using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceSystem.Api.Repositories;
using ECommerceSystem.Shared.DTOs.Models;
using ECommerceSystem.Shared.DTOs.Product;
using System.Threading.Tasks;

namespace ECommerceSystem.Api.Controllers
{
    [ApiController]
    [Route("api/user/cart")]
    [Authorize] // ✅ chỉ cho phép người dùng đã đăng nhập
   

    public class CartApiController : ControllerBase

    {
        private readonly ICartRepository _cartRepo;

        // Inject repository quản lý giỏ hàng
        public CartApiController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        /// <summary>
        /// [GET] /api/user/cart
        /// Lấy giỏ hàng hiện tại của người dùng
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            var cart = await _cartRepo.GetUserCart(); // Gọi repo để lấy giỏ hàng
            return Ok(cart);
        }

        /// <summary>
        /// [POST] /api/user/cart/items
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            var count = await _cartRepo.AddItem(request.ProductId, request.Quantity); // Thêm sản phẩm vào giỏ
            return Ok(new { cartItemCount = count }); // Trả về số lượng item trong giỏ
        }

        /// <summary>
        /// [DELETE] /api/user/cart/items/{productId}
        /// Xoá một sản phẩm ra khỏi giỏ
        /// </summary>
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var count = await _cartRepo.RemoveItem(productId); // Xoá sản phẩm theo ID
            return Ok(new { cartItemCount = count });
        }

        /// <summary>
        /// [GET] /api/user/cart/count
        /// Lấy tổng số lượng mặt hàng trong giỏ
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetCartItemCount()
        {
            var count = await _cartRepo.GetCartItemCount(); // Lấy tổng số item
            return Ok(new { cartItemCount = count });
        }

        /// <summary>
        /// [POST] /api/user/cart/checkout
        /// Tiến hành thanh toán giỏ hàng
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutModel model)
        {
            // Kiểm tra dữ liệu đầu vào hợp lệ
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Thực hiện thanh toán
            var result = await _cartRepo.DoCheckout(model);

            // Xử lý kết quả thanh toán
            if (!result)
                return BadRequest(new { success = false, message = "Checkout thất bại" });

            return Ok(new { success = true, message = "Checkout thành công" });
        }
    }
}
