namespace ECommerceSystem.Api.Hubs
{
    public interface IEcommerceHubClient
    {
        Task ReceiveOrderUpdate(int orderId, string status);
    }
}
