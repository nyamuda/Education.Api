using Education.Api.Models.Topics;

namespace Education.Api.Dtos.Topics.Subtopics;

public class SubtopicDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int TopicId { get; set; }

    public TopicDto? Topic { get; set; }

    public DateTime CreatedAt { get; set; }

    public static SubtopicDto MapFrom(Subtopic subtopic)
    {
        return new SubtopicDto
        {
            Id = subtopic.Id,
            Name = subtopic.Name,
            TopicId = subtopic.TopicId,
            CreatedAt = subtopic.CreatedAt,
        };
    }
}
