using Education.Api.Enums;

namespace Education.Api.Models.Users;

public class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public UserRole Role { get; set; } = UserRole.Student;

    public int? CurriculumId { get; set; }
    public Curriculum? Curriculum { get; set; }

    public int? ExamBoardId { get; set; }
    public ExamBoard? ExamBoard { get; set; }

    public List<Level> Levels { get; set; } = [];

    public bool IsVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
