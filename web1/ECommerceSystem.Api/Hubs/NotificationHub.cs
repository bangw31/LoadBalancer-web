using ECommerceSystem.Shared.IHub;
using Microsoft.AspNetCore.SignalR;

namespace ECommerceSystem.Api.Hubs
{
    public class NotificationHub : Hub<IEcommerceHubClient>
    {
        public override Task OnConnectedAsync()
        {
            // Handle when a client connects
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Handle when a client disconnects
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendOrderUpdate(int orderId, string status)
        {
            await Clients.All.ReceiveOrderUpdate(orderId, status);
        }
    }
}
