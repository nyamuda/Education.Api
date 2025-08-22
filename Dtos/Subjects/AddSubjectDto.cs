using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Subjects;

public class AddSubjectDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int LevelId { get; set; }
}
