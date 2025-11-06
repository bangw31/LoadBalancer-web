namespace ECommerceSystem.Shared.DTOs.User
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }
}

