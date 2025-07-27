namespace Education.Api.Models;

public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required int UserId { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
