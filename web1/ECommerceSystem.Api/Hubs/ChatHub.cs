using ECommerceSystem.Api.Data;
using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceSystem.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatConnectionManager _connManager;
        private readonly WebDBContext _db;

        public ChatHub(ChatConnectionManager connManager, WebDBContext db)
        {
            _connManager = connManager;
            _db = db;
        }

        private int GetUserId() =>
            int.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        private string? GetUserRole() =>
            Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var role = GetUserRole();

            if (userId > 0)
            {
                _connManager.AddConnection(userId, Context.ConnectionId);

                if (role == "Admin")
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId > 0)
                _connManager.RemoveConnection(userId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Customer gửi tin nhắn → Admin
        /// </summary>
        public async Task SendMessageFromCustomer(ChatMessage message)
        {
            try
            {
                var fromUserId = GetUserId();
                if (fromUserId == 0)
                    throw new Exception("Không xác định được người gửi từ JWT.");

                message.FromUserId = fromUserId;
                message.SentAt = DateTime.UtcNow;

                // Nếu chưa có ToUserId, mặc định gửi tới Admin (ID=1)
                if (message.ToUserId == 0)
                    message.ToUserId = 1;

                if (string.IsNullOrEmpty(message.Content) && string.IsNullOrEmpty(message.FileUrl))
                    throw new Exception("Tin nhắn trống.");

                _db.ChatMessages.Add(message);
                await _db.SaveChangesAsync();

                await Clients.Group("Admins").SendAsync(
                    "ReceiveMessage",
                    message.FromUserId,
                    message.Content,
                    message.SentAt,
                    message.FileUrl,
                    message.FileType,
                    message.FileName
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ChatHub] Error in SendMessageFromCustomer: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveError", ex.Message);
            }
        }

        /// <summary>
        /// Admin gửi tin nhắn → Customer
        /// </summary>
        public async Task SendMessageFromAdmin(ChatMessage message)
        {
            try
            {
                var fromUserId = GetUserId();
                if (fromUserId == 0)
                    throw new Exception("Không xác định được Admin gửi.");

                message.FromUserId = fromUserId;
                message.SentAt = DateTime.UtcNow;

                if (message.ToUserId == 0)
                    throw new Exception("Chưa chọn khách hàng để gửi.");

                _db.ChatMessages.Add(message);
                await _db.SaveChangesAsync();

                // Gửi tới khách hàng
                foreach (var connId in _connManager.GetConnections(message.ToUserId))
                {
                    await Clients.Client(connId).SendAsync(
                        "ReceiveMessage",
                        message.FromUserId,
                        message.Content,
                        message.SentAt,
                        message.FileUrl,
                        message.FileType,
                        message.FileName
                    );
                }

                // Echo lại cho admin
                foreach (var connId in _connManager.GetConnections(message.FromUserId))
                {
                    await Clients.Client(connId).SendAsync(
                        "ReceiveMessage",
                        message.FromUserId,
                        message.Content,
                        message.SentAt,
                        message.FileUrl,
                        message.FileType,
                        message.FileName
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ChatHub] Error in SendMessageFromAdmin: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveError", ex.Message);
            }
        }
    }
}
