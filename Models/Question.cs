using Education.Api.Enums;
using Education.Api.Models.Flags;
using Education.Api.Models.Topics;
using Education.Api.Models.Users;

namespace Education.Api.Models;

public class Question
{
    public int Id { get; set; }
    public required string Title { get; set; }

    // Rich text (for rendering in the frontend with formatting)
    public string? ContentHtml { get; set; }

    // Plain text (for searching, indexing, and quick filtering)
    public required string ContentText { get; set; }

    public int? Marks { get; set; }

    public required int SubjectId { get; set; }
    public Subject? Subject { get; set; }

    public int? TopicId { get; set; }
    public Topic? Topic { get; set; }

    public int? SubtopicId { get; set; }
    public Subtopic? Subtopic { get; set; }

    public required int UserId { get; set; }
    public User? User { get; set; }

    public  List<Tag> Tags { get; set; } = [];

    public List<Upvote> Upvotes { get; set; } = [];

    public List<QuestionFlag> Flags { get; set; } = [];

    public List<Comment> Comments { get; set; } = [];

    public List<Answer> Answers { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
