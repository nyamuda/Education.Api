namespace Education.Api.Services.Abstractions.Topics;

public interface ITopicService
{
    Task<SubjectDto> GetByIdAsync(int id);

    Task<PageInfo<SubjectDto>> GetAsync(int page, int pageSize);

    Task<SubjectDto> AddAsync(AddSubjectDto dto);

    Task UpdateAsync(int id, UpdateSubjectDto dto);

    Task DeleteAsync(int id);
}
