using ECommerceSystem.Api.Data.Repositories;
using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceSystem.Shared.DTOs.Models;
using ECommerceSystem.Shared.DTOs.User;

namespace ECommerceSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthController(UserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        // ==================== REGISTER ====================
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra username đã tồn tại
            var existing = await _userRepo.GetUserPasswordHash(model.UserName);
            if (existing != null)
                return BadRequest(new { error = "Tên đăng nhập đã tồn tại." });

            // Tạo user mới
            var user = new User
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                DeviceToken = ""
            };

            // Tạo user bằng Identity (Identity sẽ hash password)
            var result = await _userRepo.CreateUserAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { error = string.Join(", ", result.Errors.Select(e => e.Description)) });

            // Thêm role "User"
            var role = await _userRepo.GetRoleByName("User");
            if (role == null)
            {
                var roleResult = await _userRepo.CreateRoleAsync(new Role { Name = "User" });
                if (!roleResult.Succeeded)
                    return StatusCode(500, new { error = string.Join(", ", roleResult.Errors.Select(e => e.Description)) });
            }

            var addRoleResult = await _userRepo.AddUserToRoleAsync(user, "User");
            if (!addRoleResult.Succeeded)
                return StatusCode(500, new { error = string.Join(", ", addRoleResult.Errors.Select(e => e.Description)) });

            return Ok(new { message = "Đăng ký thành công." });
        }

        // ==================== LOGIN ====================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepo.GetUserPasswordHash(model.Username);
            if (user == null)
                return Unauthorized(new { error = "Tên đăng nhập hoặc mật khẩu không đúng." });

            var validPassword = await _userRepo.CheckPasswordAsync(user, model.Password);
            if (!validPassword)
                return Unauthorized(new { error = "Tên đăng nhập hoặc mật khẩu không đúng." });

            // Lấy thông tin user + role
            var (userDto, rolesString) = await _userRepo.GetUserInfoAndRole(user.UserName);
            if (userDto == null)
                return StatusCode(500, new { error = "Lỗi lấy thông tin người dùng." });

            // Chuẩn hóa roles
            var roles = rolesString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Tạo claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
            }

            // Nếu user là admin, chắc chắn thêm role "Admin"
            if (roles.Contains("Admin"))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            // Tạo JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                roles = roles,
                user = new
                {
                    userDto.Id,
                    userDto.Name,
                    userDto.Email
                }
            });
        }

        // ==================== GET ROLE ====================
        [HttpGet("role")]
        [Authorize]
        public async Task<IActionResult> GetUserRole()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { error = "Token không hợp lệ." });

                var user = await _userRepo.GetUserPasswordHashById(int.Parse(userIdClaim));
                if (user == null)
                    return NotFound(new { error = "Người dùng không tồn tại." });

                var (_, roles) = await _userRepo.GetUserInfoAndRole(user.UserName);
                return Ok(new { roles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Lỗi lấy vai trò: {ex.Message}" });
            }
        }

        // ==================== REFRESH TOKEN (MOCK) ====================
        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult Refresh([FromBody] RefreshTokenRequest model)
        {
            return Ok(new RefreshTokenResponse
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token"
            });
        }
    }
}
