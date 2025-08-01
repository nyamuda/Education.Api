using Education.Api.Enums;

namespace Education.Api.Dtos.Flags;

/// <summary>
/// Represents a report or flag raised by a user on an answer.
/// </summary>
public class AddAnswerFlagDto
{
    public required PostFlagType FlagType { get; set; }

    /// <summary>
    /// Optional description provided by the user when flagging the answer.
    /// This is only required when the selected flag type is <c>Other</c>,
    /// allowing the user to describe the issue in their own words.
    /// </summary>
    public string? Content { get; set; }
}
