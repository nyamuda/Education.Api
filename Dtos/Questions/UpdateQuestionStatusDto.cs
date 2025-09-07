using System.ComponentModel.DataAnnotations;
using Education.Api.Enums.Questions;

namespace Education.Api.Dtos.Questions;

public class UpdateQuestionStatusDto
{
    [Required]
    public QuestionStatus Status { get; set; }
}
