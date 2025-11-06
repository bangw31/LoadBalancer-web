
namespace ECommerceSystem.Shared.DTOs.Product
{
    public class CartDTO
    {
        public int UserId { get; set; } // hoặc string nếu dùng Guid/email
        public List<CartItemDTO> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Quantity * i.Price);
    }
}
