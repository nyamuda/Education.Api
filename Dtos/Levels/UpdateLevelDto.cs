using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Levels;

public class UpdateLevelDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int ExamBoardId { get; set; }
}
