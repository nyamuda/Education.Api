using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Subjects;

public class UpdateSubjectDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int LevelId { get; set; }
}
