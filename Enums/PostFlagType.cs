namespace Education.Api.Enums;

/// <summary>
/// Flags for inappropriate or low-quality posts (questions or answers).
/// </summary>
public enum PostFlagType
{
    /// <summary>
    /// The post is spam, such as promotional or irrelevant links.
    /// </summary>
    Spam,

    /// <summary>
    /// The post contains offensive, abusive, or inappropriate content.
    /// </summary>
    Offensive,

    /// <summary>
    /// The post is a duplicate of another existing question or answer.
    /// </summary>
    Duplicate,

    /// <summary>
    /// The post is not relevant to the topic or question.
    /// </summary>
    Irrelevant,

    /// <summary>
    /// The post violates rules but doesn't fit other categories.
    /// </summary>
    Other
}
