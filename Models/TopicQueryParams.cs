using Education.Api.Enums.Topics;

namespace Education.Api.Models;

public class TopicQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int? CurriculumId { get; set; } = null;

    public int? ExamBoardId { get; set; } = null;
    public int? LevelId { get; set; } = null;

    public int? SubjectId { get; set; } = null;

    public TopicSortOption SortBy = TopicSortOption.DateCreated;
}
