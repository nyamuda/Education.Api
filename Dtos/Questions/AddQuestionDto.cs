using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Questions;

public class AddQuestionDto
{
    [Required]
    public required string Content { get; set; }
    
    [Required]
    public required int SubjectId { get; set; }
    
    public required int TopicId { get; set; }

    public int SubtopicId { get; set; }

    public int? Marks { get; set; }

    public List<string> Tags { get; set; } = [];
}
