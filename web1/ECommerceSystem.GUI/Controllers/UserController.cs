using ECommerceSystem.GUI.Apis;
using ECommerceSystem.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserApi _userApi;

        public UserController(IUserApi userApi)
        {
            _userApi = userApi;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userApi.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userApi.GetByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _userApi.UpdateAsync(model.Id, model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _userApi.SoftDeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất 1 người dùng để xóa.";
                return RedirectToAction("Index");
            }

            await _userApi.SoftDeleteMultipleAsync(selectedIds);
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _userApi.CreateAsync(model);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Search(string name)
        {
            var results = await _userApi.SearchByNameAsync(name);
            return View("Index", results);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteMultiple(List<string> selectedIds)
        //{
        //    if (selectedIds == null || !selectedIds.Any())
        //    {
        //        TempData["Error"] = "Vui lòng chọn ít nhất 1 người dùng để xóa.";
        //        return RedirectToAction("Index");
        //    }

        //    await _userApi.SoftDeleteMultipleAsync(selectedIds);
        //    return RedirectToAction("Index");
        //}

    }
}
