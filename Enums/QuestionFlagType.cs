namespace Education.Api.Enums;

/// <summary>
/// Flags for inappropriate or low-quality questions.
/// </summary>
public enum QuestionFlagType
{
    /// <summary>
    /// The question is spam, such as promotional or irrelevant links.
    /// </summary>
    Spam,

    /// <summary>
    /// The question contains offensive, abusive, or inappropriate content.
    /// </summary>
    Offensive,

    /// <summary>
    /// The question is a duplicate of another existing question.
    /// </summary>
    Duplicate,

    /// <summary>
    /// The question is not relevant to the topic or subject.
    /// </summary>
    Irrelevant,

    /// <summary>
    /// The question violates rules but doesn't fit other categories.
    /// </summary>
    Other
}
