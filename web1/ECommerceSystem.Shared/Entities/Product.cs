namespace ECommerceSystem.Shared.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int CategoryId { get; set; }
        public int? Stock { get; set; }
        public float? Rating { get; set; }
        public bool IsPromoted { get; set; }
        public string? QrCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        
        public string? Slug { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}