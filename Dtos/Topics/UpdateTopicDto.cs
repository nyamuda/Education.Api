using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Topics;

public class UpdateTopicDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int SubjectId { get; set; }
}
