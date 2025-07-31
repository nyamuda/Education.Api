using Education.Api.Dtos.Curriculums;
using Education.Api.Models;

namespace Education.Api.Dtos.ExamBoards;

public class ExamBoardDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int CurriculumId { get; set; }
    public CurriculumDto? Curriculum { get; set; }

    public DateTime CreatedAt { get; set; }

    public static ExamBoardDto MapFrom(ExamBoard examBoard)
    {
        return new ExamBoardDto
        {
            Id = examBoard.Id,
            Name = examBoard.Name,
            CurriculumId = examBoard.CurriculumId,
            Curriculum =
                examBoard.Curriculum != null
                    ? new CurriculumDto
                    {
                        Id = examBoard.Curriculum.Id,
                        Name = examBoard.Curriculum.Name
                    }
                    : null,
            CreatedAt = examBoard.CreatedAt,
        };
    }
}
