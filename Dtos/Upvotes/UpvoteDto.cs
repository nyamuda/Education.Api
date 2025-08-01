using Education.Api.Dtos.Users;
using Education.Api.Models;

namespace Education.Api.Dtos.Upvotes;

public class UpvoteDto
{
    public required int Id { get; set; }

    public required int UserId { get; set; }
    public UserDto? User { get; set; }

    public int? QuestionId { get; set; }

    public int? AnswerId { get; set; }

    public int? CommentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public static UpvoteDto MapFrom(Upvote upvote)
    {
        return new UpvoteDto
        {
            Id = upvote.Id,
            UserId = upvote.UserId,
            User =
                upvote.User != null
                    ? new UserDto { Id = upvote.User.Id, Username = upvote.User.Username }
                    : null,
            QuestionId = upvote.QuestionId,
            AnswerId = upvote.AnswerId,
            CommentId = upvote.CommentId,
            CreatedAt = upvote.CreatedAt,
        };
    }
}
