using ECommerceSystem.Shared.DTOs.Models;
using ECommerceSystem.Shared.DTOs.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSystem.Api.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int productId, int qty = 1);
        Task<int> RemoveItem(int productId);
        Task<CartDTO> GetUserCart();
        Task<CartDTO> GetCart(string userId);
        Task<int> GetCartItemCount(string userId = "");
        Task<bool> DoCheckout(CheckoutModel model);
    }
}
