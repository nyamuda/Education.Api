using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Subjects;
using Education.Api.Dtos.Tags;
using Education.Api.Dtos.Topics;
using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Dtos.Users;

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

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
