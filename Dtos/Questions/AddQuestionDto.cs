using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Questions;

public class AddQuestionDto
{
    [Required]
    public required string ContentText { get; set; }

    [Required]
    public required int SubjectId { get; set; }

    public required int TopicId { get; set; }

    public int SubtopicId { get; set; }

    public int? Marks { get; set; }

    // Rich text (for rendering in the frontend with formatting)
    public string? AnswerHtml { get; set; }

    // Plain text (for searching, indexing, and quick filtering)
    public string? AnswerText { get; set; }

    public List<string> Tags { get; set; } = [];
}
