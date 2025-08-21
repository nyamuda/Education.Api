using Education.Api.Models.Topics;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Question> Questions { get; set; } = [];
    public List<Topic> Topics { get; set; } = [];

    public int LevelId { get; set; }
    public Level? Level { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
