using Education.Api.Enums.Subjects;

namespace Education.Api.Models;

public class SubjectQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int? CurriculumId { get; set; } = null;

    public int? ExamBoardId { get; set; } = null;
    public int? LevelId { get; set; } = null;

    public SubjectSortOption SortBy = SubjectSortOption.DateCreated;
}
