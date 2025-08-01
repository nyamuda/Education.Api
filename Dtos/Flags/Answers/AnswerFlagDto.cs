using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Users;
using Education.Api.Enums;
using Education.Api.Models.Flags;

namespace Education.Api.Dtos.Flags.Answers;

public class AnswerFlagDto
{
    public required int Id { get; set; }
    public string? Content { get; set; }

    /// <summary>
    /// ID of the user who submitted the flag.
    /// </summary>
    public required int UserId { get; set; }

    public UserDto? User { get; set; }

    /// <summary>
    /// The ID of the answer being flagged.
    /// </summary>
    public required int AnswerId { get; set; }
    public AnswerDto? Answer { get; set; }

    /// <summary>
    /// The current status of the flag (e.g., pending review, resolved).
    /// </summary>
    public required FlagStatus Status { get; set; }

    public required DateTime CreatedAt { get; set; }

    public static AnswerFlagDto MapFrom(AnswerFlag answerFlag)
    {
        return new AnswerFlagDto
        {
            Id = answerFlag.Id,
            Content = answerFlag.Content,
            UserId = answerFlag.UserId,
            User =
                answerFlag.User != null
                    ? new UserDto { Id = answerFlag.User.Id, Username = answerFlag.User.Username, }
                    : null,
            AnswerId = answerFlag.AnswerId,
            Status = answerFlag.Status,
            CreatedAt = answerFlag.CreatedAt,
        };
    }
}
