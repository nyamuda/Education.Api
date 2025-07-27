using Education.Api.Enums;
using Education.Api.Models.Users;

namespace Education.Api.Models.Flags;

/// <summary>
/// Represents a flag raised by a user on a comment, typically for moderation purposes.
/// </summary>
public class CommentFlag
{
    public int Id { get; set; }

    /// <summary>
    /// Optional description provided by the user when flagging a comment.
    /// This is only required when the selected <see cref="CommentFlagType"/> is <c>Other</c>,
    /// allowing the user to describe the issue in their own words.
    /// </summary>

    public string? Content { get; set; }

    /// <summary>
    /// ID of the user who flagged the comment.
    /// </summary>
    public required int UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// ID of the comment that was flagged.
    /// </summary>
    public required int CommentId { get; set; }
    public Comment? Comment { get; set; }

    /// <summary>
    /// The reason/type of the flag.
    /// </summary>
    public required CommentFlagType FlagType { get; set; }

    /// <summary>
    /// The current status of the flag (e.g., pending review, resolved).
    /// </summary>
    public FlagStatus Status { get; set; } = FlagStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
