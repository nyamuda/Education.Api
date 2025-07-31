using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Topics.Subtopics;

public class UpdateSubtopicDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int TopicId { get; set; }
}
