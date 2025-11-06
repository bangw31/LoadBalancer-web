using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ECommerceSystem.Api.Data
{
    public static class AdminInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // ✅ Tạo 2 role: Admin, User
            string[] roles = { "Admin", "User" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new Role { Name = roleName };
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                        Console.WriteLine($"✅ Đã tạo role: {roleName}");
                    else
                        Console.WriteLine($"❌ Lỗi tạo role {roleName}: {string.Join(", ", result.Errors)}");
                }
            }

            // ✅ Tạo tài khoản admin mặc định
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "Super Admin",
                    EmailConfirmed = true,   // ⚡ Quan trọng
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("✅ Đã tạo user admin và gán vào role Admin");
                }
                else
                {
                    Console.WriteLine($"❌ Không thể tạo admin: {string.Join(", ", result.Errors)}");
                }
            }
            else
            {
                Console.WriteLine("ℹ️ Admin đã tồn tại, bỏ qua bước tạo.");
            }
        }
    }
}

