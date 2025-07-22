using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Auth;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public required string UserName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [RegularExpression(
        @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain a mix of letters, numbers, and special characters"
    )]
    public required string Password { get; set; }
}
