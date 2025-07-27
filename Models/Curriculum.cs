using Microsoft.EntityFrameworkCore;

namespace Education.Api.Models;

[Index(nameof(Name), IsUnique = true)]
public class Curriculum
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
