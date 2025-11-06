    namespace ECommerceSystem.Shared.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string DeviceToken { get; set; }
        public bool IsDeleted { get; set; }
    }
}

