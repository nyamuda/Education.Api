using Education.Api.Models;

namespace Education.Api.Dtos.Curriculums;

public class CurriculumDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }

    public static CurriculumDto MapFrom(Curriculum curriculum)
    {
        return new CurriculumDto
        {
            Id = curriculum.Id,
            Name = curriculum.Name,
            CreatedAt = curriculum.CreatedAt
        };
    }
}
