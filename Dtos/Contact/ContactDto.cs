using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Contact;

public class ContactDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public string? Message { get; set; }
}
