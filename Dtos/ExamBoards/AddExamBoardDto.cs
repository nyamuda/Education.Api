using System.ComponentModel.DataAnnotations;
using Education.Api.Models;

namespace Education.Api.Dtos.ExamBoards;

public class AddExamBoardDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int CurriculumId { get; set; }
}
