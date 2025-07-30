using System.ComponentModel.DataAnnotations;
using Education.Api.Models;

namespace Education.Api.Dtos.Subjects;

public class SubjectDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static SubjectDto MapFrom(Subject subject)
    {
        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            CreatedAt = subject.CreatedAt,
        };
    }
}
