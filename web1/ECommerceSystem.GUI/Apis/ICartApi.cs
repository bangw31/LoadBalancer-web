using ECommerceSystem.Shared.DTOs.Models;
using ECommerceSystem.Shared.DTOs.Product;
using Refit;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Apis
{
    public interface ICartApi
    {
        // GET api/user/cart
        [Get("/api/user/cart")]
        Task<CartDTO> GetCart();

        // POST api/user/cart/items
        [Post("/api/user/cart/items")]
        Task<ApiResponse<object>> AddItem([Body] AddToCartRequest request);

        // DELETE api/user/cart/items/{productId}
        [Delete("/api/user/cart/items/{productId}")]
        Task<ApiResponse<object>> RemoveItem(int productId);

        // GET api/user/cart/count
        [Get("/api/user/cart/count")]
        Task<ApiResponse<int>> GetCartItemCount();

        // POST api/user/cart/checkout
        [Post("/api/user/cart/checkout")]
        Task<ApiResponse<object>> Checkout([Body] CheckoutModel model);
    }
}
