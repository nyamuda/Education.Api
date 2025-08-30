using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Answers;

public class AddAnswerDto
{
    [Required]
    public required string ContentHtml { get; set; }

    [Required]
    public required string ContentText { get; set; }
}
