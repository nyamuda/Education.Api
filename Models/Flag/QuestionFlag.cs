using Education.Api.Enums;

namespace Education.Api.Models.Flags;

/// <summary>
/// Represents a report or flag raised by a user on a question.
/// </summary>
public class QuestionFlag : BaseFlag
{
    /// <summary>
    /// The ID of the question being flagged, if applicable.
    /// </summary>
    public required int QuestionId { get; set; }
    public Question? Question { get; set; }

    /// <summary>
    /// The type or category of the flag (e.g., Spam, Offensive).
    /// </summary>
    public required QuestionFlagType FlagType { get; set; }
}
