using Education.Api.Enums;

namespace Education.Api.Models.Flags;

/// <summary>
/// Represents a report or flag raised by a user on an answer.
/// </summary>
public class AnswerFlag : BaseFlag
{
    /// <summary>
    /// The ID of the answer being flagged.
    /// </summary>
    public required int AnswerId { get; set; }
    public Answer? Answer { get; set; }

    /// <summary>
    /// The type or category of the flag (e.g., Spam, Offensive).
    /// </summary>
    public required PostFlagType FlagType { get; set; }
}
