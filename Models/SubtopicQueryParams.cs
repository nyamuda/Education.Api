using Education.Api.Enums.Subtopics;

namespace Education.Api.Models;

public class SubtopicQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int? CurriculumId { get; set; } = null;

    public int? ExamBoardId { get; set; } = null;
    public int? LevelId { get; set; } = null;

    public int? SubjectId { get; set; } = null;

    public int? TopicId { get; set; } = null;

    public SubtopicSortOption SortBy = SubtopicSortOption.DateCreated;
}
