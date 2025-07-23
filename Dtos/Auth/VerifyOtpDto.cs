using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Auth;

/// <summary>
/// Represents the data required to verify a one-time password (OTP)
/// during email verification or password reset processes.
/// </summary>
public class VerifyOtpDto
{
    /// <summary>
    /// The one-time password (OTP) sent to the user's email address.
    /// Must be exactly 6 characters.
    /// </summary>
    [Required]
    [MinLength(6)]
    [MaxLength(6)]
    public string Otp { get; set; } = default!;

    /// <summary>
    /// The email address associated with the OTP verification request.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}
