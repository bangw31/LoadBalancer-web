using ECommerceSystem.GUI.Apis;
using ECommerceSystem.Shared.DTOs.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Controllers
{
    //[Authorize] // Yêu cầu JWT cho tất cả action
   
    
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryApi _categoryApi;

        public CategoryController(ICategoryApi categoryApi)
        {
            _categoryApi = categoryApi;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryApi.GetAllAsync();
            return View(categories);
        }

        public async Task<IActionResult> CreateOrUpdate(int? id)
        {
            if (id.HasValue)
            {
                var category = await _categoryApi.GetByIdAsync(id.Value);
                ViewBag.IsEdit = true;
                return View(category);
            }
            ViewBag.IsEdit = false;
            return View(new CategoryDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(CategoryDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (model.Id > 0)
                    await _categoryApi.UpdateAsync(model.Id, model);
                else
                    await _categoryApi.CreateAsync(model);
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Lỗi khi lưu danh mục.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryApi.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "Lỗi khi xóa danh mục.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                await _categoryApi.DeleteAllAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "Lỗi khi xóa tất cả danh mục.";
                return RedirectToAction("Index");
            }
        }

    }
}