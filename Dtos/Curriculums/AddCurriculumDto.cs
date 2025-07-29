using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Curriculums;

public class AddCurriculumDto
{
    [Required]
    public required string Name { get; set; }
}
