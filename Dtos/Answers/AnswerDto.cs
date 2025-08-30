using Education.Api.Dtos.Questions;
using Education.Api.Dtos.Upvotes;
using Education.Api.Dtos.Users;
using Education.Api.Models;

namespace Education.Api.Dtos.Answers;

public class AnswerDto
{
    public required int Id { get; set; }

    public required string ContentText { get; set; }
    public required string ContentHtml { get; set; }

    public required int QuestionId { get; set; }

    public required int UserId { get; set; }

    public UserDto? User { get; set; }

    public required List<UpvoteDto> Upvotes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public static AnswerDto MapFrom(Answer answer)
    {
        return new AnswerDto
        {
            Id = answer.Id,
            ContentText = answer.ContentText,
            ContentHtml = answer.ContentHtml,
            QuestionId = answer.QuestionId,
            UserId = answer.UserId,
            User =
                answer.User != null
                    ? new UserDto { Id = answer.User.Id, Username = answer.User.Username, }
                    : null,
            Upvotes = answer
                .Upvotes
                .Select(
                    upv =>
                        new UpvoteDto
                        {
                            Id = upv.Id,
                            UserId = upv.UserId,
                            AnswerId = upv.AnswerId
                        }
                )
                .ToList(),
            CreatedAt = answer.CreatedAt,
            UpdatedAt = answer.UpdatedAt
        };
    }
}
