using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Curriculums;

public class UpdateCurriculumDto
{
    [Required]
    public required string Name { get; set; }
}
