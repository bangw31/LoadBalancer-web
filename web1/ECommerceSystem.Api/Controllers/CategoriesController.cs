using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceSystem.Api.Data;
using ECommerceSystem.Shared.Entities;
using ECommerceSystem.Shared.DTOs;
using ECommerceSystem.Shared.DTOs.Category;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceSystem.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [AllowAnonymous] // Cho phép truy cập công khai (GET danh mục)
    [Route("api/public/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly WebDBContext _dbContext;

        // Inject DbContext
        public CategoriesController(WebDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// [GET] /api/public/categories
        /// Trả về danh sách tất cả danh mục sản phẩm
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _dbContext.Categories
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }

        /// <summary>
        /// [GET] /api/public/categories/get/{id}
        /// Trả về thông tin chi tiết của một danh mục theo Id
        /// </summary>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _dbContext.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// [POST] /api/public/categories/Create
        /// Tạo mới một danh mục
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO dto)
        {
            // Kiểm tra model hợp lệ
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto == null)
                return BadRequest("CategoryDTO is null");

            // Tạo entity từ DTO
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            // Trả về object đã tạo cùng với đường dẫn truy cập
            dto.Id = category.Id;
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, dto);
        }

        /// <summary>
        /// [PUT] /api/public/categories/edit/{id}
        /// Cập nhật thông tin danh mục
        /// </summary>
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO dto)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            // Cập nhật thông tin
            category.Name = dto.Name;
            category.Description = dto.Description;
            await _dbContext.SaveChangesAsync();

            return NoContent(); // 204
        }

        /// <summary>
        /// [DELETE] /api/public/categories/delete/{id}
        /// Xóa một danh mục theo ID
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
