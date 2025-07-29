namespace Education.Api.Dtos.Curriculums;

public class CurriculumDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
