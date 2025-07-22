using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Auth;

//OTP verification
//Used to verify email address or reset password
public class OtpVerificationDto
{
    [Required]
    [MinLength(6)]
    [MaxLength(6)]
    public string OtpCode { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}
