using System.ComponentModel.DataAnnotations;
using Education.Api.Enums.Questions;

namespace Education.Api.Dtos.Questions;

public class AddQuestionDto
{
    [Required]
    public required string Title { get; set; }

    [Required]
    // Plain text (for searching, indexing, and quick filtering)
    public required string QuestionText { get; set; }

    // Rich text (for rendering in the frontend with formatting)
    public string? QuestionHtml { get; set; }

    [Required]
    public required int SubjectId { get; set; }

    public int? TopicId { get; set; }

    public int? SubtopicId { get; set; }

    public int? Marks { get; set; }

    // Rich text (for rendering in the frontend with formatting)
    public string? AnswerHtml { get; set; }

    // Plain text (for searching, indexing, and quick filtering)
    public string? AnswerText { get; set; }

    [Required]
    [MinLength(
        1,
        ErrorMessage = "Include at least one tag so others can easily find your question."
    )]
    public List<string> Tags { get; set; } = [];

    public QuestionStatus Status { get; set; } = QuestionStatus.Draft;
}
