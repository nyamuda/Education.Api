using Education.Api.Models;

namespace Education;

public class Like
{
    public int Id { get; set; }

    public required int UserId { get; set; }
    public User? User { get; set; }

    public int? QuestionId { get; set; }
    public Question? Question { get; set; }
}
