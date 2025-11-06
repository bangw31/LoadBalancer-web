// EcommerceSystem.API/Controllers/CommentsController.cs
using EcommerceSystem.API.Data.Repositories;
using ECommerceSystem.Shared.DTOs;
using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentRepository _commentRepo;

        public CommentsController(CommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        // ============== helpers ==============
        private static CommentDto ToDto(Comment c) => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            UserName = c.User?.UserName ?? "User",
            CreatedAt = c.CreatedAt
        };

        private int? GetUserId()
        {
            // thử nhiều claim type phổ biến
            var claim = User.FindFirst("nameid")
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("sub");
            if (claim == null) return null;
            return int.TryParse(claim.Value, out var id) ? id : null;
        }

        // ============== GET: comments by product ==============
        [HttpGet("product/{productId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentsByProductId(int productId)
        {
            var comments = await _commentRepo.GetCommentsByProductIdAsync(productId);
            // luôn trả 200 với []
            return Ok(comments.Select(ToDto).ToList());
        }

        // ============== GET: comment by id ==============
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var comment = await _commentRepo.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(ToDto(comment));
        }

        // ============== POST: add (login required) ==============
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto dto)
        {
            if (dto == null) return BadRequest("Comment data is required.");
            if (dto.ProductId <= 0) return BadRequest("Valid ProductId is required.");
            if (string.IsNullOrWhiteSpace(dto.Content)) return BadRequest("Comment content is required.");

            var userId = GetUserId();
            if (userId is null or <= 0) return Unauthorized();

            var entity = new Comment
            {
                ProductId = dto.ProductId,
                Content = dto.Content.Trim(),
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var added = await _commentRepo.AddCommentAsync(entity);

            // đảm bảo repo load kèm User nếu cần (Include/Load Reference)
            var result = ToDto(added);

            return CreatedAtAction(nameof(GetCommentById), new { id = result.Id }, result);
        }

        // ============== PUT: update (login required) ==============
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto dto)
        {
            if (dto == null) return BadRequest("Comment data is required.");

            var existing = await _commentRepo.GetCommentByIdAsync(id);
            if (existing == null) return NotFound();

            var userId = GetUserId();
            if (userId is null or <= 0) return Unauthorized();
            if (existing.UserId != userId.Value) return Forbid();

            existing.Content = dto.Content?.Trim() ?? existing.Content;
            existing.UpdatedAt = DateTime.UtcNow;

            await _commentRepo.UpdateCommentAsync(existing);
            return Ok(ToDto(existing));
        }

        // ============== DELETE: (login required) ==============
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var existing = await _commentRepo.GetCommentByIdAsync(id);
            if (existing == null) return NotFound();

            var userId = GetUserId();
            if (userId is null or <= 0) return Unauthorized();
            if (existing.UserId != userId.Value) return Forbid();

            await _commentRepo.DeleteCommentAsync(id);
            return Ok(new { message = "Comment deleted successfully." });
        }

        // ============== GET: all (Admin only) ==============
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentRepo.GetAllCommentsAsync();
            return Ok(comments.Select(ToDto).ToList());
        }
    }
}
