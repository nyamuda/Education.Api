namespace Education.Api.Enums;

/// <summary>
/// Flags for inappropriate or unnecessary comments.
/// </summary>
public enum CommentFlagType
{
    /// <summary>
    /// The comment contains harassment, hate speech, or abuse.
    /// </summary>
    HarassmentOrAbuse,

    /// <summary>
    /// The comment is rude, sarcastic, or condescending in tone.
    /// </summary>
    UnfriendlyOrRude,

    /// <summary>
    /// The comment does not add value or is unnecessary (e.g., "thanks").
    /// </summary>
    NotNeeded,

    /// <summary>
    /// The comment is spam or contains promotional content.
    /// </summary>
    Spam,

    /// <summary>
    /// The comment doesn't fit other categories but should be flagged.
    /// </summary>
    Other
}
