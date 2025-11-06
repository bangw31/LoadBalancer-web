using EcommerceSystem.API.Services;
using ECommerceSystem.Api.Data.Repositories;
using ECommerceSystem.Shared.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSystem.Api.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    [AllowAnonymous] // Cho phép truy cập công khai (GET danh mục)
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// [GET] /api/admin/users
        /// Lấy danh sách toàn bộ người dùng
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// [GET] /api/admin/users/{id}
        /// Lấy thông tin chi tiết của một người dùng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// [PUT] /api/admin/users/{id}
        /// Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("Id không khớp");

            await _userService.UpdateAsync(id, dto);
            return NoContent(); // 204
        }

        /// <summary>
        /// [DELETE] /api/admin/users/{id}
        /// Xóa mềm người dùng theo ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _userService.SoftDeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// [POST] /api/admin/users
        /// Tạo mới một người dùng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDTO dto)
        {
            await _userService.CreateAsync(dto); // Yêu cầu đã triển khai trong UserService
            return Ok();
        }

        /// <summary>
        /// [GET] /api/admin/users/search?name=abc
        /// Tìm kiếm người dùng theo tên
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<UserDTO>>> SearchByName([FromQuery] string name)
        {
            var users = await _userService.SearchByNameAsync(name);
            return Ok(users);
        }

        /// <summary>
        /// [POST] /api/admin/users/delete-multiple
        /// Xóa mềm nhiều người dùng cùng lúc theo danh sách ID
        /// </summary>
        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            await _userService.SoftDeleteMultipleAsync(ids);
            return NoContent(); // 204
        }
    }
}
