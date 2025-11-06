using ECommerceSystem.Api.Data;
using ECommerceSystem.Api.Hubs;
using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSystem.Api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : ControllerBase
    {
        private readonly WebDBContext _db;
        private readonly IHubContext<ChatHub> _hub;
        private readonly ChatConnectionManager _connManager;

        public ChatController(WebDBContext db, IHubContext<ChatHub> hub, ChatConnectionManager connManager)
        {
            _db = db;
            _hub = hub;
            _connManager = connManager;
        }

        // ✅ Lấy UserId từ JWT token
        private int GetMyId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("nameid")?.Value;

            return int.TryParse(userIdClaim, out var id) ? id : 0;
        }

        // ✅ Upload file gửi qua chat
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null)
                return BadRequest("No file uploaded");

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            var fileType = file.ContentType.StartsWith("image/") ? "image" :
                           file.ContentType.StartsWith("video/") ? "video" : "file";

            return Ok(new { url = fileUrl, fileName = file.FileName, fileType });
        }

        // ✅ User gửi tin nhắn cho Admin (Admin ID = 1)
        [HttpPost("customer/send")]
        public async Task<IActionResult> CustomerSend([FromBody] ChatMessage message)
        {
            var me = GetMyId();
            if (me == 0)
                return Unauthorized("Không xác định được người dùng.");

            message.FromUserId = me;
            message.ToUserId = 1; // luôn gửi đến Admin duy nhất
            message.SentAt = DateTime.UtcNow;

            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();

            // Gửi realtime tới Admin group
            await _hub.Clients.Group("Admins").SendAsync("ReceiveMessage", message);

            return Ok(message);
        }

        // ✅ Admin gửi tin nhắn cho 1 user
        [HttpPost("admin/send")]
        [Authorize] // chỉ cần token hợp lệ (vì chỉ có 1 admin thật)
        public async Task<IActionResult> AdminSend([FromBody] ChatMessage message)
        {
            var adminId = GetMyId();
            if (adminId != 1)
                return Forbid("Bạn không phải admin.");

            message.FromUserId = adminId;
            message.SentAt = DateTime.UtcNow;

            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();

            // Gửi tin tới User (client)
            foreach (var connId in _connManager.GetConnections(message.ToUserId))
            {
                await _hub.Clients.Client(connId).SendAsync("ReceiveMessage", new
                {
                    fromUserId = message.FromUserId,
                    toUserId = message.ToUserId,
                    content = message.Content,
                    sentAt = message.SentAt
                });
            }

            // Gửi lại cho admin (hiển thị realtime)
            foreach (var connId in _connManager.GetConnections(1))
            {
                await _hub.Clients.Client(connId).SendAsync("ReceiveMessage", message);
            }

            return Ok(message);
        }

        // ✅ Lấy danh sách tất cả user (trừ admin)
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _db.Users
                .Where(u => u.Id != 1)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName
                })
                .ToListAsync();

            return Ok(users);
        }

        // ✅ Lấy lịch sử chat
        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] int withUserId = 0)
        {
            var myId = GetMyId();
            if (myId == 0) return Unauthorized("Không xác định được người dùng.");

            bool isAdmin = User.IsInRole("Admin");
            IQueryable<ChatMessage> query;

            if (isAdmin)
            {
                if (withUserId == 0) return BadRequest("Thiếu userId cần xem lịch sử.");
                const int adminId = 1;
                query = _db.ChatMessages.Where(m =>
                    (m.FromUserId == withUserId && m.ToUserId == adminId) ||
                    (m.FromUserId == adminId && m.ToUserId == withUserId)
                );
            }
            else
            {
                const int adminId = 1;
                query = _db.ChatMessages.Where(m =>
                    (m.FromUserId == myId && m.ToUserId == adminId) ||
                    (m.FromUserId == adminId && m.ToUserId == myId)
                );
            }

            var messages = await query
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Id,
                    fromUserId = m.FromUserId,   // <-- added here
                    toUserId = m.ToUserId,       // <-- added here
                    m.Content,
                    m.FileUrl,
                    m.FileType,
                    m.FileName,
                    SentAt = m.SentAt.ToLocalTime()
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}
