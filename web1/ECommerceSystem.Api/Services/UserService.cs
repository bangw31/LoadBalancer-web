using EcommerceSystem.API.Services;
using ECommerceSystem.Shared.DTOs.User;
using ECommerceSystem.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceSystem.Api.Data.Repositories;

namespace ECommerceSystem.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepo;

        public UserService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task CreateAsync(UserDTO dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                DeviceToken = dto.DeviceToken,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            // Replace with actual password hashing logic
            await _userRepo.CreateUserAsync(user, "hashed_password");
        }

        public Task<List<UserDTO>> GetAllAsync() => _userRepo.GetAllAsync();

        public Task<UserDTO> GetByIdAsync(int id) => _userRepo.GetByIdAsync(id);

        public Task UpdateAsync(int id, UserDTO dto) => _userRepo.UpdateAsync(id, dto);

        public Task SoftDeleteAsync(int id) => _userRepo.SoftDeleteAsync(id);

        public Task<List<UserDTO>> SearchByNameAsync(string name) => _userRepo.SearchByNameAsync(name);

        public Task SoftDeleteMultipleAsync(List<int> ids) => _userRepo.SoftDeleteMultipleAsync(ids);
    }
}
