using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Dtos.Subjects;
using Education.Api.Dtos.Tags;
using Education.Api.Dtos.Topics;
using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Dtos.Upvotes;
using Education.Api.Dtos.Users;
using Education.Api.Enums.Questions;
using Education.Api.Models;

namespace Education.Api.Dtos.Questions;

public class QuestionDto
{
    public required int Id { get; set; }
    public required string ContentText { get; set; }
    public string? ContentHtml { get; set; }
    public int? Marks { get; set; }

    public required int SubjectId { get; set; }
    public SubjectDto? Subject { get; set; }

    public int? TopicId { get; set; }
    public TopicDto? Topic { get; set; }

    public int? SubtopicId { get; set; }
    public SubtopicDto? Subtopic { get; set; }

    public required int UserId { get; set; }
    public UserDto? User { get; set; }

    // <summary>
    /// The answer provided by the author of the question, if they chose to answer their own question.
    /// This is distinct from other user-submitted answers and may be null if the author did not provide one.
    /// </summary>
    public AnswerDto? AuthorAnswer { get; set; }

    public List<TagDto> Tags { get; set; } = [];

    public List<UpvoteDto> Upvotes { get; set; } = [];

    public List<CommentDto> Comments { get; set; } = [];

    public QuestionStatus? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static QuestionDto MapFrom(Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            ContentText = question.ContentText,
            ContentHtml = question.ContentHtml,
            Status = question.Status,
            SubjectId = question.SubjectId,
            TopicId = question.TopicId,
            UserId = question.UserId,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };
    }
}
