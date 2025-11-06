using ECommerceSystem.Shared.DTOs.User;
using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Api.Data.Repositories
{
    public class UserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserRepository(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ✅ Tạo user mới
        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        // ✅ Thêm user vào role
        public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        // ✅ Lấy user theo username (dùng login)
        public async Task<User?> GetUserPasswordHash(string username)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted);
        }

        // ✅ Lấy user theo Id
        public async Task<User?> GetUserPasswordHashById(int id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        // ✅ Kiểm tra password
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        // ✅ Lấy role theo tên
        public async Task<Role?> GetRoleByName(string roleName)
        {
            return await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        // ✅ Tạo role mới
        public async Task<IdentityResult> CreateRoleAsync(Role role)
        {
            return await _roleManager.CreateAsync(role);
        }

        // ✅ Lấy thông tin user + role
        public async Task<(UserDTO?, string)> GetUserInfoAndRole(string username)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted);
            if (user == null) return (null, null);

            var roles = await _userManager.GetRolesAsync(user);

            var dto = new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DeviceToken = user.DeviceToken,
                IsDeleted = user.IsDeleted
            };

            return (dto, roles.FirstOrDefault() ?? "");
        }

        // ✅ Lấy tất cả user
        public async Task<List<UserDTO>> GetAllAsync()
        {
            return await _userManager.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    DeviceToken = u.DeviceToken,
                    IsDeleted = u.IsDeleted
                }).ToListAsync();
        }

        // ✅ Lấy user theo Id
        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DeviceToken = user.DeviceToken,
                IsDeleted = user.IsDeleted
            };
        }

        // ✅ Cập nhật user
        public async Task UpdateAsync(int id, UserDTO dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) throw new Exception("Người dùng không tồn tại.");

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.DeviceToken = dto.DeviceToken;
            user.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        // ✅ Xóa mềm (soft delete)
        public async Task SoftDeleteAsync(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new Exception("Người dùng không tồn tại.");

            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);
        }

        // ✅ Xóa mềm nhiều user
        public async Task SoftDeleteMultipleAsync(List<int> ids)
        {
            var users = await _userManager.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                user.IsDeleted = true;
                await _userManager.UpdateAsync(user);
            }
        }

        // ✅ Tìm user theo tên
        public async Task<List<UserDTO>> SearchByNameAsync(string name)
        {
            return await _userManager.Users
                .Where(u => u.Name.Contains(name) && !u.IsDeleted)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    DeviceToken = u.DeviceToken,
                    IsDeleted = u.IsDeleted
                }).ToListAsync();
        }
    }
}
