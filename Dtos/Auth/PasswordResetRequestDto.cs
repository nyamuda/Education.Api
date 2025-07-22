using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Auth;

/// <summary>
/// Represents the data required to request password reset.
/// </summary>
public class PasswordResetRequestDto
{
    /// <summary>
    /// The user's email address. Must be in a valid email format.
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
