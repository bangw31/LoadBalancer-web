using Microsoft.AspNetCore.Identity;

namespace ECommerceSystem.Shared.Entities
{
    public class User : IdentityUser<int> // Use int as the key type
    {
        public string Name { get; set; }
        public string? DeviceToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Role : IdentityRole<int> // Use int as the key type
    {
       
    }
}