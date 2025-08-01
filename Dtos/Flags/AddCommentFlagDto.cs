using Education.Api.Enums;

namespace Education.Api.Dtos.Flags;

public class AddCommentFlagDto
{
    public required CommentFlagType FlagType { get; set; }

    /// <summary>
    /// Optional description provided by the user when flagging the comment.
    /// This is only required when the selected flag type is <c>Other</c>,
    /// allowing the user to describe the issue in their own words.
    /// </summary>
    public string? Content { get; set; }
}
