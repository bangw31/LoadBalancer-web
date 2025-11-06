using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ECommerceSystem.Api.Hubs
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Lấy ID từ Claim của JWT
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
