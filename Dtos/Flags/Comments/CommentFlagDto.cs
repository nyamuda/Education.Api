using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Users;
using Education.Api.Enums;
using Education.Api.Models.Flags;

namespace Education.Api.Dtos.Flags.Comments;

public class CommentFlagDto
{
    public required int Id { get; set; }
    public string? Content { get; set; }

    /// <summary>
    /// ID of the user who submitted the flag.
    /// </summary>
    public required int UserId { get; set; }

    public UserDto? User { get; set; }

    /// <summary>
    /// The ID of the comment being flagged.
    /// </summary>
    public required int CommentId { get; set; }
    public CommentDto? Comment { get; set; }

    /// <summary>
    /// The current status of the flag (e.g., pending review, resolved).
    /// </summary>
    public required FlagStatus Status { get; set; }

    public required DateTime CreatedAt { get; set; }

    public static CommentFlagDto MapFrom(CommentFlag commentFlag)
    {
        return new CommentFlagDto
        {
            Id = commentFlag.Id,
            Content = commentFlag.Content,
            UserId = commentFlag.UserId,
            User =
                commentFlag.User != null
                    ? new UserDto
                    {
                        Id = commentFlag.User.Id,
                        Username = commentFlag.User.Username,
                    }
                    : null,
            CommentId = commentFlag.CommentId,
            Status = commentFlag.Status,
            CreatedAt = commentFlag.CreatedAt,
        };
    }
}
