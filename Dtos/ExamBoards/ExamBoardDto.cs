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
}
