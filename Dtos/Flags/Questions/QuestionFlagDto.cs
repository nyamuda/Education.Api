using Education.Api.Dtos.Questions;
using Education.Api.Dtos.Users;
using Education.Api.Enums;
using Education.Api.Models.Flags;

namespace Education.Api.Dtos.Flags.Questions;

public class QuestionFlagDto
{
    public required int Id { get; set; }
    public string? Content { get; set; }

    /// <summary>
    /// ID of the user who submitted the flag.
    /// </summary>
    public required int UserId { get; set; }

    public UserDto? User { get; set; }

    /// <summary>
    /// The ID of the question being flagged.
    /// </summary>
    public required int QuestionId { get; set; }
    public QuestionDto? Question { get; set; }

    /// <summary>
    /// The current status of the flag (e.g., pending review, resolved).
    /// </summary>
    public required FlagStatus Status { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required QuestionFlagType FlagType { get; set; }

    public static QuestionFlagDto MapFrom(QuestionFlag questionFlag)
    {
        return new QuestionFlagDto
        {
            Id = questionFlag.Id,
            Content = questionFlag.Content,
            UserId = questionFlag.UserId,
            User =
                questionFlag.User != null
                    ? new UserDto
                    {
                        Id = questionFlag.User.Id,
                        Username = questionFlag.User.Username,
                    }
                    : null,
            QuestionId = questionFlag.QuestionId,
            Status = questionFlag.Status,
            FlagType = questionFlag.FlagType,
            CreatedAt = questionFlag.CreatedAt,
        };
    }
}
