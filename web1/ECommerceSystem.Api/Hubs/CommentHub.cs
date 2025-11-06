using ECommerceSystem.Api.Data;
using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Api.Hubs
{
    [Authorize] // Chỉ user đã login mới được connect
    public class CommentHub : Hub
    {
        private readonly WebDBContext _db;

        public CommentHub(WebDBContext db)
        {
            _db = db;
        }

        // Khi user join nhóm sản phẩm (để nhận comment realtime)
        public async Task JoinProductGroup(int productId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"product-{productId}");
        }

        // Khi user gửi comment
        public async Task SendComment(Comment comment)
        {
            // Lấy userId từ token, tránh client gửi giả
            var userIdStr = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                throw new HubException("Không xác định được user.");

            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // load thông tin user để hiển thị
            var dbComment = await _db.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            // gửi comment realtime cho tất cả client trong product group
            await Clients.Group($"product-{comment.ProductId}")
                         .SendAsync("ReceiveComment", new
                         {
                             id = dbComment.Id,
                             content = dbComment.Content,
                             createdAt = dbComment.CreatedAt,
                             userName = dbComment.User.UserName
                         });
        }
    }
}
