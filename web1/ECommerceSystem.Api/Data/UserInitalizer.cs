using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ECommerceSystem.Api.Data
{
    public static class UserInitializer
    {
        public static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // ✅ Đảm bảo role "User" tồn tại
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new Role { Name = "User" });
                Console.WriteLine("✅ Đã tạo role User");
            }

            // ✅ Tạo user mặc định
            var userEmail = "user@gmail.com";
            var defaultUser = await userManager.FindByEmailAsync(userEmail);

            if (defaultUser == null)
            {
                defaultUser = new User
                {
                    UserName = userEmail, // dùng email cho đồng bộ
                    Email = userEmail,
                    Name = "Default User",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(defaultUser, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, "User");
                    Console.WriteLine("✅ Đã tạo user mặc định (user@gmail.com / User@123)");
                }
                else
                {
                    throw new Exception($"❌ Không thể tạo user mặc định: {string.Join(", ", result.Errors)}");
                }   
            }
            else
            {
                Console.WriteLine("ℹ️ User mặc định đã tồn tại.");
            }
        }
    }
}
