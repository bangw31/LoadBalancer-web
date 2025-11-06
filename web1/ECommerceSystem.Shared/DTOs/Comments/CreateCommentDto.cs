namespace ECommerceSystem.Shared.DTOs;

public class CreateCommentDto
{
    public int ProductId { get; set; }
    public string Content { get; set; } = default!;
}
