using Education.Api.Dtos.Subjects;
using Education.Api.Models.Topics;

namespace Education.Api.Dtos.Topics;

public class TopicDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int SubjectId { get; set; }
    public SubjectDto? Subject { get; set; }
    public DateTime CreatedAt { get; set; }

    public static TopicDto MapFrom(Topic topic)
    {
        return new TopicDto
        {
            Id = topic.Id,
            Name = topic.Name,
            SubjectId = topic.SubjectId,
            CreatedAt = topic.CreatedAt
        };
    }
}
