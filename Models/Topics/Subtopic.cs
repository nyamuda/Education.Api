namespace Education.Api.Models.Topics;

public class Subtopic
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required int TopicId { get; set; }

    public Topic? Topic { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
