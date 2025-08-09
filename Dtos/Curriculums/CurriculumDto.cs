using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Models;

namespace Education.Api.Dtos.Curriculums;

public class CurriculumDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public List<ExamBoardDto> ExamBoards { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    public static CurriculumDto MapFrom(Curriculum curriculum)
    {
        return new CurriculumDto
        {
            Id = curriculum.Id,
            Name = curriculum.Name,
            ExamBoards = curriculum
                .ExamBoards
                .Select(
                    x =>
                        new ExamBoardDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            CurriculumId = x.CurriculumId,
                            Levels = x.Levels
                                .Select(
                                    l =>
                                        new LevelDto
                                        {
                                            Id = l.Id,
                                            Name = l.Name,
                                            ExamBoardId = l.ExamBoardId
                                        }
                                )
                                .ToList()
                        }
                )
                .ToList(),
            CreatedAt = curriculum.CreatedAt
        };
    }
}
