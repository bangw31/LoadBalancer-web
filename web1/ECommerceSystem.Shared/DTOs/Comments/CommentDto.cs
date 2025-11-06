namespace ECommerceSystem.Shared.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
