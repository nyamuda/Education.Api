using Microsoft.EntityFrameworkCore;

namespace Education.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Tag
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<Question> Questions { get; set; } = [];
}
