using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Auth;

public class ResetPasswordDto
{
    [Required]
    public required string ResetToken { get; set; }

    [Required]
    [RegularExpression(
        @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain a mix of letters, numbers, and special characters"
    )]
    public required string Password { get; set; } //the new password
}
