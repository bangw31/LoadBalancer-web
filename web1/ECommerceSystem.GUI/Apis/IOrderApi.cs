using ECommerceSystem.Shared.DTOs.Product;
using Refit;

namespace ECommerceSystem.GUI.Apis
{
    public interface IOrderApi
    {
        [Post("/api/user/orders/create")]
        Task<ApiResponse<OrderDTO>> CreateOrderAsync([Body] CreateOrderRequest request);
        
        
        
        
        [Get("/api/admin/orders")]
        Task<List<OrderDTO>> GetAllOrdersAsync();

        [Get("/api/admin/orders/{id}")]
        Task<OrderDTO> GetOrderByIdAsync(int id);

        [Post("/api/admin/orders/{id}/update-status")]
        Task<ApiResponse<object>> UpdateOrderStatusAsync(int id, [Body] string status);



    }
}
