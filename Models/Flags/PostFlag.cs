using Education.Api.Enums;

namespace Education.Api.Models.Flags;

/// <summary>
/// Represents a report or flag raised by a user on a post (question or answer).
/// </summary>
public class PostFlag
{
    public int Id { get; set; }

    /// <summary>
    /// Optional description provided by the user when flagging a post.
    /// This is only required when the selected <see cref="PostFlagType"/> is <c>Other</c>,
    /// allowing the user to describe the issue in their own words.
    public string? Content { get; set; }

    /// <summary>
    /// The ID of the user who created the flag.
    /// </summary>
    public required int UserId { get; set; }
    public User? User { get; set; }

    public int? QuestionId { get; set; }
    public Question? Question { get; set; }

    /// <summary>
    /// The type or category of the flag (e.g., Spam, Offensive).
    /// </summary>
    public required PostFlagType FlagType { get; set; }

    /// <summary>
    /// The current status of the flag (e.g., Pending, Resolved, Rejected).
    /// </summary>
    public FlagStatus Status { get; set; } = FlagStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
