namespace Education.Api.Models.Topics;

public class Topic
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public required int CurriculumId { get; set; }

    public Curriculum? Curriculum { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
