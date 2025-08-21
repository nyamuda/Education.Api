using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Subjects;

public class AddSubjectDto
{
    [Required]
    public required string Name { get; set; }
    
    [Required]
    public required int ExamBoardId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Please select at least one educational level.")]
    public List<int> LevelIds { get; set; } = [];
}
