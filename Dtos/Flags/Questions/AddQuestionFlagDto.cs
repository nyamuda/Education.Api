using Education.Api.Enums;

namespace Education.Api.Dtos.Flags;

/// <summary>
/// Represents a report or flag raised by a user on a question.
/// </summary>
public class AddQuestionFlagDto
{
    public required QuestionFlagType FlagType { get; set; }

    /// <summary>
    /// Optional description provided by the user when flagging the question.
    /// This is only required when the selected flag type is <c>Other</c>,
    /// allowing the user to describe the issue in their own words.
    /// </summary>
    public string? Content { get; set; }
}
