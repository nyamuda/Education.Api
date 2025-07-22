using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Auth;

public class EmailVerificationRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}
