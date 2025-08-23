using Education.Api.Dtos.Topics;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Topics;

public interface ITopicService
{
    Task<TopicDto> GetByIdAsync(int id);

    Task<PageInfo<TopicDto>> GetAsync(TopicQueryParams queryParams);

    Task<TopicDto> AddAsync(AddTopicDto dto);

    Task UpdateAsync(int id, UpdateTopicDto dto);

    Task DeleteAsync(int id);
}
