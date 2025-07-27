using Microsoft.EntityFrameworkCore;

namespace Education.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Curriculum> Curriculums { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
