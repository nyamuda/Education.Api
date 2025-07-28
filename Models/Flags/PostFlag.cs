using Education.Api.Enums;

namespace Education.Api.Models.Flags;

/// <summary>
/// Represents a report or flag raised by a user on a post (question or answer).
/// </summary>
public class PostFlag : BaseFlag
{
    /// <summary>
    /// The ID of the question being flagged, if applicable.
    /// </summary>
    public int? QuestionId { get; set; }
    public Question? Question { get; set; }

    /// <summary>
    /// The ID of the answer being flagged, if applicable.
    /// </summary>
    public int? AnswerId { get; set; }
    public Answer? Answer { get; set; }

    /// <summary>
    /// The type or category of the flag (e.g., Spam, Offensive).
    /// </summary>
    public required PostFlagType FlagType { get; set; }
}
