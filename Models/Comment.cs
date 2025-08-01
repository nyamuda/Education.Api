using Education.Api.Models.Flags;
using Education.Api.Models.Users;

namespace Education.Api.Models;

/// <summary>
/// Represents a comment made by a user, either on a question or an answer.
/// Each comment is linked to a user and optionally to either a question or an answer.
/// </summary>
public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required int UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// If this comment is on a question, the ID of the related question.
    /// </summary>
    public int? QuestionId { get; set; }
    public Question? Question { get; set; }

    /// <summary>
    /// If this comment is on an answer, the ID of the related answer.
    /// </summary>
    public int? AnswerId { get; set; }
    public Answer? Answer { get; set; }

    public List<CommentFlag> Flags { get; set; } = [];
    public List<Upvote> Upvotes { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
