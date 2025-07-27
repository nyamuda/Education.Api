namespace Education.Api.Models;

public class Upvote
{
    public int Id { get; set; }

    public required int UserId { get; set; }
    public User? User { get; set; }

    public int? QuestionId { get; set; }
    public Question? Question { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
