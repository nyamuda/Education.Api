using Education.Api.Dtos.Comments;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Subjects;
using Education.Api.Dtos.Tags;
using Education.Api.Dtos.Topics;
using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Dtos.Upvotes;
using Education.Api.Dtos.Users;
using Education.Api.Models;

namespace Education.Api.Dtos.Questions;

public class QuestionDto
{
    public required int Id { get; set; }
    public required string Content { get; set; }

    public int? Marks { get; set; }

    public required int ExamBoardId { get; set; }
    public ExamBoardDto? ExamBoard { get; set; }

    public required int SubjectId { get; set; }
    public SubjectDto? Subject { get; set; }

    public required int TopicId { get; set; }
    public TopicDto? Topic { get; set; }

    public List<SubtopicDto> Subtopics { get; set; } = [];

    public required int UserId { get; set; }
    public UserDto? User { get; set; }

    public List<TagDto> Tags { get; set; } = [];

    public required List<UpvoteDto> Upvotes { get; set; }

    public List<CommentDto> Comments { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static QuestionDto MapFrom(Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            Content = question.Content,
            ExamBoardId = question.ExamBoardId,
            SubjectId = question.SubjectId,
            TopicId = question.TopicId,
            UserId = question.UserId,
            Upvotes = question
                .Upvotes
                .Select(
                    upv =>
                        new UpvoteDto
                        {
                            Id = upv.Id,
                            UserId = upv.UserId,
                            QuestionId = upv.QuestionId,
                        }
                )
                .ToList(),
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };
    }
}
