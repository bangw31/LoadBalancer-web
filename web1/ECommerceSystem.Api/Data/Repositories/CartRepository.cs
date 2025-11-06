using ECommerceSystem.Api.Data;
using ECommerceSystem.Api.Repositories;
using ECommerceSystem.Shared.DTOs.Models;
using ECommerceSystem.Shared.DTOs.Product;
using ECommerceSystem.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceSystem.Api.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly WebDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(WebDBContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int productId, int qty)
        {
            int userId = GetUserId();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId.ToString());
                if (cart == null)
                {
                    cart = new ShoppingCart { UserId = userId.ToString() };
                    _db.ShoppingCarts.Add(cart);
                    await _db.SaveChangesAsync();
                }

                var cartItem = await _db.CartDetails.FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var product = await _db.Products.FindAsync(productId);
                    if (product == null)
                        throw new InvalidOperationException("Product not found");

                    cartItem = new CartDetail
                    {
                        ProductId = productId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = product.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return await GetCartItemCount(userId.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [CartRepository] AddItem Error: " + ex.Message);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> RemoveItem(int productId)
        {
            int userId = GetUserId();

            try
            {
                var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId.ToString());
                if (cart == null)
                    throw new InvalidOperationException("Invalid cart");

                var cartItem = await _db.CartDetails.FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);
                if (cartItem == null)
                    throw new InvalidOperationException("No item in cart");

                if (cartItem.Quantity <= 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity--;

                await _db.SaveChangesAsync();
                return await GetCartItemCount(userId.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [CartRepository] RemoveItem Error: " + ex.Message);
                throw;
            }
        }

    

        public async Task<CartDTO> GetCart(string userId)
        {
            if (!int.TryParse(userId, out var id))
                throw new InvalidOperationException("Invalid user ID");

            return await BuildCartDTO(id);
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            int id = string.IsNullOrEmpty(userId) ? GetUserId() : int.Parse(userId);

            var count = await _db.CartDetails
                .Where(cd => cd.ShoppingCart.UserId == id.ToString())
                .CountAsync();

            return count;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                int userId = GetUserId();

                var cart = await _db.ShoppingCarts
                    .Include(c => c.CartDetails)
                    .FirstOrDefaultAsync(c => c.UserId == userId.ToString());

                if (cart == null || cart.CartDetails.Count == 0)
                    throw new InvalidOperationException("Cart is empty");

                var order = new Order
                {
                    UserId = userId,
                    Total = cart.CartDetails.Sum(i => i.Quantity * i.UnitPrice),
                    Status = "Pending",
                    DeliveryLocation = model.Address,
                    QrCode = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = cart.CartDetails.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.UnitPrice
                    }).ToList()
                };

                _db.Orders.Add(order);
                _db.CartDetails.RemoveRange(cart.CartDetails);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [CartRepository] DoCheckout Error: " + ex.Message);
                await transaction.RollbackAsync();
                return false;
            }
        }

        private int GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            if (principal == null || !principal.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User not authenticated");

            Console.WriteLine("🧾 JWT Claims:");
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"➡️ {claim.Type}: {claim.Value}");
            }

            var stringId =
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                principal.FindFirst("sub")?.Value ??
                principal.FindFirst("nameidentifier")?.Value;

            Console.WriteLine($"✅ Claim NameIdentifier tìm được: {stringId}");

            if (!int.TryParse(stringId, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing user ID claim");

            return userId;
        }


        public async Task<CartDTO> GetUserCart()
        {
            try
            {
                int userId = GetUserId(); // ✅ dùng đúng cách lấy ID

                var cart = await _db.ShoppingCarts
                    .Include(c => c.CartDetails)
                    .ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId.ToString());

                if (cart == null)
                {
                    return new CartDTO
                    {
                        UserId = userId,
                        Items = new List<CartItemDTO>()
                    };
                }

                return new CartDTO
                {
                    UserId = userId,
                    Items = cart.CartDetails.Select(d => new CartItemDTO
                    {
                        ProductId = d.ProductId,
                        Name = d.Product?.Name ?? "",
                        Quantity = d.Quantity,
                        Price = d.UnitPrice
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [CartRepository] GetUserCart Error: " + ex.Message);
                throw;
            }
        }

        private async Task<CartDTO> BuildCartDTO(int userId)
        {
            var cart = await _db.ShoppingCarts
                .Include(c => c.CartDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.ToString());

            if (cart == null)
                return new CartDTO { UserId = userId, Items = new List<CartItemDTO>() };

            return new CartDTO
            {
                UserId = userId,
                Items = cart.CartDetails.Select(d => new CartItemDTO
                {
                    ProductId = d.ProductId,
                    Name = d.Product?.Name ?? "",
                    Quantity = d.Quantity,
                    Price = d.UnitPrice
                }).ToList()
            };
        }
    }
}
