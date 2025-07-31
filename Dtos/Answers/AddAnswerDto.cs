using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Answers;

public class AddAnswerDto
{
    [Required]
    public required string Content { get; set; }

    [Required]
    public required int QuestionId { get; set; }
}
