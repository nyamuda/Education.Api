using Education.Api.Models.Users;

namespace Education.Api.Models;

public class QuestionBookmark
{
    public int Id { get; set; }

    public required int QuestionId { get; set; }
    public Question? Question { get; set; }
    public required int UserId { get; set; }

    public User? User { get; set; }
}
