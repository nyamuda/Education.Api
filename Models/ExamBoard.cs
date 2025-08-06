using Microsoft.EntityFrameworkCore;

namespace Education.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class ExamBoard
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Level> Levels { get; set; } = [];
    public required int CurriculumId { get; set; }
    public Curriculum? Curriculum { get; set; }

    public List<Subject> Subjects { get; set; } = [];

    public List<Question> Questions { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
