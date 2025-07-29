using Education.Api.Enums;
using Education.Api.Models.Users;

namespace Education.Api.Models.Flags;

/// <summary>
/// Represents a flag raised by a user on a comment, typically for moderation purposes.
/// </summary>
public class CommentFlag : BaseFlag
{
    /// <summary>
    /// ID of the comment that was flagged.
    /// </summary>
    public required int CommentId { get; set; }
    public Comment? Comment { get; set; }

    /// <summary>
    /// The reason/type of the flag.
    /// </summary>
    public required CommentFlagType FlagType { get; set; }
}
