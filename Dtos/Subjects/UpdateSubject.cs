using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Subjects;

public class UpdateSubjectDto
{
    [Required]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Please select at least one curriculum.")]
    public List<int> CurriculumIds { get; set; } = [];

    [Required(ErrorMessage = "Please select at least one exam board.")]
    public List<int> ExamBoardIds { get; set; } = [];
}
