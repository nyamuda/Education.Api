using Education.Api.Enums;
using Education.Api.Models.Users;

namespace Education.Api.Models.Flags;

/// <summary>
/// Base class for a user-submitted flag on a piece of content (e.g., comment or post).
/// Contains shared properties for all flag types.
/// </summary>
public abstract class BaseFlag
{
    public int Id { get; set; }

    /// <summary>
    /// Optional description provided by the user when flagging the content.
    /// This is only required when the selected flag type is <c>Other</c>,
    /// allowing the user to describe the issue in their own words.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// ID of the user who submitted the flag.
    /// </summary>
    public required int UserId { get; set; }

    public User? User { get; set; }

    /// <summary>
    /// The current status of the flag (e.g., pending review, resolved).
    /// </summary>
    public FlagStatus Status { get; set; } = FlagStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
