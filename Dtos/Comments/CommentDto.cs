using Education.Api.Dtos.Users;
using Education.Api.Models;

namespace Education.Api.Dtos.Comments;

/// <summary>
/// Represents a comment made by a user, either on a question or an answer.
/// Each comment is linked to a user and optionally to either a question or an answer.
/// </summary>
public class CommentDto
{
    public required int Id { get; set; }
    public required string Content { get; set; }
    public required int UserId { get; set; }
    public UserDto? User { get; set; }

    /// <summary>
    /// If this comment is on a question, the ID of the related question.
    /// </summary>
    public int? QuestionId { get; set; }

    /// <summary>
    /// If this comment is on an answer, the ID of the related answer.
    /// </summary>
    public int? AnswerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public static CommentDto MapFrom(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            User =
                comment.User != null
                    ? new UserDto { Id = comment.User.Id, Username = comment.User.Username }
                    : null,
            AnswerId = comment.AnswerId,
            QuestionId = comment.QuestionId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}
