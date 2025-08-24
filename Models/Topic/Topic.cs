using Microsoft.EntityFrameworkCore;

namespace Education.Api.Models.Topics;

[Index(nameof(Name), IsUnique = false)]
public class Topic
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public List<Subtopic> Subtopics { get; set; } = [];

    public List<Question> Questions { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
