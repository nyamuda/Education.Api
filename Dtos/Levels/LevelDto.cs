using System.ComponentModel.DataAnnotations;
using Education.Api.Models;

namespace Education.Api.Dtos.Levels;

public class LevelDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int ExamBoardId { get; set; }
    public ExamBoard? ExamBoard { get; set; }
    public DateTime CreatedAt { get; set; }

    public static LevelDto MapFrom(Level level)
    {
        return new LevelDto
        {
            Id = level.Id,
            Name = level.Name,
            ExamBoardId = level.ExamBoardId,
            ExamBoard = level.ExamBoard,
            CreatedAt = level.CreatedAt,
        };
    }
}
