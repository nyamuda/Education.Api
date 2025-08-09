using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Enums;
using Education.Api.Models.Users;

namespace Education.Api.Dtos.Users;

public class UserDto
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public string? Email { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsVerified { get; set; }

    public int? CurriculumId { get; set; }
    public CurriculumDto? Curriculum { get; set; }

    public int? ExamBoardId { get; set; }
    public ExamBoardDto? ExamBoard { get; set; }

    public List<LevelDto> Levels { get; set; } = [];

    public DateTime? CreatedAt { get; set; }

    public static UserDto MapFrom(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsVerified = user.IsVerified,
            CurriculumId = user.CurriculumId,
            Curriculum =
                user.Curriculum != null
                    ? new CurriculumDto { Id = user.Curriculum.Id, Name = user.Curriculum.Name }
                    : null,
            ExamBoardId = user.ExamBoardId,
            ExamBoard =
                user.ExamBoard != null
                    ? new ExamBoardDto
                    {
                        Id = user.ExamBoard.Id,
                        Name = user.ExamBoard.Name,
                        CurriculumId = user.ExamBoard.CurriculumId
                    }
                    : null,
            Levels = user.Levels
                .Select(
                    l =>
                        new LevelDto
                        {
                            Id = l.Id,
                            Name = l.Name,
                            ExamBoardId = l.ExamBoardId
                        }
                )
                .ToList(),
            CreatedAt = user.CreatedAt,
        };
    }
}
