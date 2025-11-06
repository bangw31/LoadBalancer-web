using ECommerceSystem.Shared.DTOs.Models;
using ECommerceSystem.Shared.DTOs.Product;
using ECommerceSystem.Shared.DTOs.User;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Apis
{
    // Auth API
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<LoginResponse> Login([Body] LoginModel model);

        [Post("/api/auth/register")]
        Task<RegisterResponse> Register([Body] RegisterModel model);

        [Get("/api/auth/role")]
        Task<RoleResponse> GetCurrentRole();

        [Post("/api/auth/refresh")]
        Task<RefreshTokenResponse> Refresh([Body] RefreshTokenRequest model);
    }

    // Admin API
    public interface IAdminApi
    {
        [Get("/api/admin/statistics")]
        Task<StatisticDTO> GetStatisticsAsync([Query] string type, [Query] string? period = null);

        [Get("/api/admin/inventory")]
        Task<ApiInventoryResponse> GetInventoryAsync();

        [Get("/api/admin/user-activity")]
        Task<ApiUserActivityResponse> GetUserActivityAsync();
    }

    public class ApiInventoryResponse
    {
        public List<object> LowStock { get; set; } = new();
    }

    public class ApiUserActivityResponse
    {
        public List<object> Activities { get; set; } = new();
    }

    // User API
    public interface IUserApi
    {
        [Get("/api/admin/users")]
        Task<List<UserDTO>> GetAllAsync();

        [Get("/api/admin/users/{id}")]
        Task<UserDTO> GetByIdAsync(int id);

        [Post("/api/admin/users")]
        Task CreateAsync([Body] UserDTO dto);

        [Put("/api/admin/users/{id}")]
        Task UpdateAsync(int id, [Body] UserDTO dto);

        [Delete("/api/admin/users/{id}")]
        Task SoftDeleteAsync(int id);

        [Post("/api/admin/users/delete-multiple")]
        Task SoftDeleteMultipleAsync([Body] List<int> ids);

        [Get("/api/admin/users/search")]
        Task<List<UserDTO>> SearchByNameAsync(string name);
    }
}
