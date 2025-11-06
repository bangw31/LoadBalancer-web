using ECommerceSystem.GUI.Apis;
using ECommerceSystem.GUI.Models;
using ECommerceSystem.Shared.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ECommerceSystem.GUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryApi _categoryApi;
        private readonly IProductApi _productApi;   

        public HomeController(ILogger<HomeController> logger, ICategoryApi categoryApi, IProductApi productApi)
        {
            _logger = logger;
            _categoryApi = categoryApi;
            _productApi = productApi;
        }

        // Trang chủ - có thể lọc theo danh mục
        public async Task<IActionResult> Index(int? categoryId = null, int page = 1)
        {
            int pageSize = 9;

            // Lấy danh sách danh mục
            var categories = await _categoryApi.GetAllAsync();

            // Lấy danh sách sản phẩm theo category (nếu có)
            var productResponse = await _productApi.GetProductsAsync(
                search: null,
                categoryId: categoryId,
                minPrice: null,
                maxPrice: null,
                sortBy: null,
                promotion: null,
                page: page,
                pageSize: pageSize
            );

            ViewBag.Categories = categories;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)productResponse.Total / pageSize);
            ViewBag.SelectedCategoryId = categoryId;

            return View(productResponse.Products);
        }

        // Trang chính sách quyền riêng tư
        public IActionResult Privacy()
        {
            return View();
        }

        // Trang lỗi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
