using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Dtos.Users;
using Education.Api.Services.Abstractions.Users;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Users;

public class UserService(ApplicationDbContext context) : IUserService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<UserDto> GetByIdAsync(int id)
    {
        return await _context
                .Users
                .Select(
                    u =>
                        new UserDto
                        {
                            Id = u.Id,
                            Username = u.Username,
                            Email = u.Email,
                            Role = u.Role,
                            IsVerified = u.IsVerified,
                            CurriculumId = u.CurriculumId,
                            Curriculum =
                                u.Curriculum != null
                                    ? new CurriculumDto
                                    {
                                        Id = u.Curriculum.Id,
                                        Name = u.Curriculum.Name
                                    }
                                    : null,
                            ExamBoardId = u.ExamBoardId,
                            ExamBoard =
                                u.ExamBoard != null
                                    ? new ExamBoardDto
                                    {
                                        Id = u.ExamBoard.Id,
                                        Name = u.ExamBoard.Name,
                                        CurriculumId = u.ExamBoard.CurriculumId
                                    }
                                    : null,
                            Levels = u.Levels
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
                            CreatedAt = u.CreatedAt
                        }
                )
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id.Equals(id))
            ?? throw new KeyNotFoundException($@"User with ID ""{id}"" does not exist");
    }
}
