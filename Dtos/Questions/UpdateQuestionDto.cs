using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Questions;

public class UpdateQuestionDto
{
    [Required]
    public required string Content { get; set; }

    [Required]
    public required int TopicId { get; set; }

    public List<int> SubtopicIds { get; set; } = [];

    public int? Marks { get; set; }

    public List<string> Tags { get; set; } = [];
}
