namespace Education.Api.Enums;

/// <summary>
/// Flags for inappropriate or low-quality answers.
/// </summary>
public enum AnswerFlagType
{
    /// <summary>
    /// The answer is spam, such as promotional or irrelevant links.
    /// </summary>
    Spam,

    /// <summary>
    /// The answer contains offensive, abusive, or inappropriate content.
    /// </summary>
    Offensive,

    /// <summary>
    /// The answer is a duplicate of another existing answer.
    /// </summary>
    Duplicate,

    /// <summary>
    /// The answer is not relevant to the topic or question.
    /// </summary>
    Irrelevant,

    /// <summary>
    /// The answer violates rules but doesn't fit other categories.
    /// </summary>
    Other
}
