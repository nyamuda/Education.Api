namespace Education.Api.Enums;

/// <summary>
/// Represents the moderation status of a flag.
/// </summary>
public enum FlagStatus
{
    /// <summary>
    /// The flag is pending review by a moderator.
    /// </summary>
    Pending,

    /// <summary>
    /// The flag has been reviewed and accepted; action was taken.
    /// </summary>
    Resolved,

    /// <summary>
    /// The flag was reviewed and dismissed as invalid or not actionable.
    /// </summary>
    Rejected
}
