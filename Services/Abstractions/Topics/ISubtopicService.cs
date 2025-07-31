using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Topics;

public interface ISubtopicService
{
    Task<SubtopicDto> GetByIdAsync(int id);

    Task<PageInfo<SubtopicDto>> GetAsync(int page, int pageSize);

    Task<SubtopicDto> AddAsync(AddSubtopicDto dto);

    Task UpdateAsync(int id, UpdateSubtopicDto dto);

    Task DeleteAsync(int id);
}
