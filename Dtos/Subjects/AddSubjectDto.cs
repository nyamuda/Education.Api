using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Subjects;

public class AddSubjectDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Please select at least one exam board.")]
    public List<int> ExamBoardIds { get; set; } = [];
}
