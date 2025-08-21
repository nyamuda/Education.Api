using Education.Api.Enums;
using Education.Api.Models.Flags;
using Education.Api.Models.Topics;
using Education.Api.Models.Users;

namespace Education.Api.Models;

public class Question
{
    public int Id { get; set; }
    public required string Content { get; set; }

    public int? Marks { get; set; }

    public int LevelId { get; set; }
    public Level? Level { get; set; }

    public required int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public required int TopicId { get; set; }
    public Topic? Topic { get; set; }

    public List<Subtopic> Subtopics { get; set; } = [];

    public required int UserId { get; set; }
    public User? User { get; set; }

    public List<Tag> Tags { get; set; } = [];

    public List<Upvote> Upvotes { get; set; } = [];

    public List<QuestionFlag> Flags { get; set; } = [];

    public List<Comment> Comments { get; set; } = [];

    public List<Answer> Answers { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
