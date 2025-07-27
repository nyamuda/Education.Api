using Education.Api.Enums;

namespace Education.Api.Models;

/// <summary>
/// Represents a report or flag raised by a user on a post (question, or answer),
/// including the reason for the flag and optional additional context.
/// </summary>
public class PostFlag
{
    public int Id { get; set; }

    /// <summary>
    /// Optional explanation or additional details provided by the user about the flag.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// The ID of the user who created the flag.
    /// </summary>
    public required int UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// The type or category of the flag (e.g., Spam, Offensive).
    /// </summary>
    public required PostFlagType FlagType { get; set; }
}
