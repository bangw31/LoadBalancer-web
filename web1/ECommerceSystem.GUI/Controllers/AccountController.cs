using ECommerceSystem.GUI.Services;
using ECommerceSystem.Shared.DTOs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Gọi AuthService, trả về tuple (success, role, token)
            var (success, role, token) = await _authService.LoginAsync(model); // Đảm bảo phương thức trả về đúng 3 giá trị

            if (!success)
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View(model);
            }

            // Lấy thông tin user từ JWT hoặc AuthService
            var currentUser = _authService.GetCurrentUser();
            if (currentUser != null)
            {
                HttpContext.Session.SetString("UserId", currentUser.Id);
                HttpContext.Session.SetString("UserName", currentUser.Name ?? "");
                HttpContext.Session.SetString("UserRole", currentUser.Role ?? "");
            }

            // Trả về cùng Login.cshtml với token + user info
            ViewBag.Token = token;
            ViewBag.UserId = currentUser?.Id;
            ViewBag.UserName = currentUser?.Name;
            ViewBag.Role = role;

            return View("Login"); // Không tạo view mới
        }

        // Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            Response.Cookies.Delete("AuthToken");

            return RedirectToAction("Login", "Account");
        }

        // Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                // Log các lỗi nếu ModelState không hợp lệ
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        _logger.LogWarning("Lỗi trường {Field}: {Error}", entry.Key, error.ErrorMessage);
                    }
                }

                // Thông báo lỗi nếu ModelState không hợp lệ
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                // Gọi dịch vụ đăng ký người dùng
                var success = await _authService.RegisterAsync(model);

                if (!success)
                {
                    // Nếu đăng ký thất bại, log cảnh báo và hiển thị thông báo lỗi
                    _logger.LogWarning("Đăng ký thất bại cho username: {Username}", model.UserName);
                    ViewBag.ErrorMessage = "Đăng ký thất bại. Vui lòng thử lại.";
                    return View(model);
                }

                // Đăng ký thành công, log thông tin
                _logger.LogInformation("Đăng ký thành công cho username: {Username}", model.UserName);

                // Tự động đăng nhập sau khi đăng ký
                var (loginSuccess, role, _) = await _authService.LoginAsync(new LoginModel
                {
                    Username = model.UserName,
                    Password = model.Password
                });

                if (loginSuccess)
                {
                    // Đăng nhập thành công, log thông tin và điều hướng về trang chủ
                    _logger.LogInformation("Tự động đăng nhập thành công cho username: {Username}", model.UserName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Đăng nhập thất bại, log cảnh báo và yêu cầu người dùng đăng nhập thủ công
                    _logger.LogWarning("Tự động đăng nhập thất bại cho username: {Username}", model.UserName);
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
            }
            catch (ApiException apiEx)
            {
                // Xử lý lỗi từ API
                _logger.LogError($"Lỗi API khi đăng ký cho username: {model.UserName}. Lỗi: {apiEx.Message}");
                ViewBag.ErrorMessage = $"Lỗi khi tạo tài khoản từ API: {apiEx.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi bất thường
                _logger.LogError($"Lỗi không xác định khi đăng ký cho username: {model.UserName}. Lỗi: {ex.Message}");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau.";
                return View(model);
            }
        }
    }
}
