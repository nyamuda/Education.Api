using Education.Api.Models.Flags;
using Education.Api.Models.Users;

namespace Education.Api.Models;

public class Answer
{
    public int Id { get; set; }

    public required string Content { get; set; }

    public required int QuestionId { get; set; }

    public Question? Question { get; set; }

    public required int UserId { get; set; }

    public User? User { get; set; }

    public List<Upvote> Upvotes { get; set; } = [];
    public List<Like> Likes { get; set; } = [];

    public List<Comment> Comments { get; set; } = [];

    public List<AnswerFlag> Flags { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
