using Education.Api.Enums;
using Education.Api.Models.Flags;

namespace Education.Api.Models;

public class Question
{
    public int Id { get; set; }
    public required string Content { get; set; }

    public int? Marks { get; set; }

    public required int SubjectId { get; set; }

    public Subject? Subject { get; set; }

    public List<Tag> Tags { get; set; } = [];
    public List<Like> Likes { get; set; } = [];
    public List<Upvote> Upvotes { get; set; } = [];

    public List<PostFlag> Flags { get; set; } = [];
    public List<Curriculum> Curriculums { get; set; } = [];
    public List<ExamBoard> ExamBoards { get; set; } = [];

    public List<Answer> Answers { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
