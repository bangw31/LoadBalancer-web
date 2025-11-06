using ECommerceSystem.Api.Data.Repositories;
using ECommerceSystem.Shared.DTOs.User;
using ECommerceSystem.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceSystem.API.Services
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllAsync();
        Task<UserDTO> GetByIdAsync(int id);
        Task UpdateAsync(int id, UserDTO dto);
        Task SoftDeleteAsync(int id);
        Task CreateAsync(UserDTO dto);
        Task<List<UserDTO>> SearchByNameAsync(string name);
        Task SoftDeleteMultipleAsync(List<int> ids);
    }
}
