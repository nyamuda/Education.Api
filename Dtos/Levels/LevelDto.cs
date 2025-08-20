using System.ComponentModel.DataAnnotations;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Subjects;
using Education.Api.Models;

namespace Education.Api.Dtos.Levels;

public class LevelDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int ExamBoardId { get; set; }
    public ExamBoardDto? ExamBoard { get; set; }
    public List<SubjectDto> Subjects { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    public static LevelDto MapFrom(Level level)
    {
        return new LevelDto
        {
            Id = level.Id,
            Name = level.Name,
            ExamBoardId = level.ExamBoardId,
            ExamBoard =
                level.ExamBoard != null
                    ? new ExamBoardDto
                    {
                        Id = level.ExamBoard.Id,
                        Name = level.ExamBoard.Name,
                        CurriculumId = level.ExamBoard.CurriculumId
                    }
                    : null,
            CreatedAt = level.CreatedAt,
        };
    }
}
