using Education.Api.Dtos.Topics;
using Education.Api.Models;
using Education.Api.Models.Topics;

namespace Education.Api.Services.Abstractions.Topics;

public interface ITopicService
{
    Task<TopicDto> GetByIdAsync(int id);

    Task<PageInfo<TopicDto>> GetAsync(TopicQueryParams queryParams);

    Task<TopicDto> AddAsync(AddTopicDto dto);

    Task AddTopicsToSubjectAsync(int subjectId, List<Topic> topics);

    Task UpdateAsync(int id, UpdateTopicDto dto);

    Task DeleteAsync(int id);

    Task<List<Topic>> DeserializeTopicsFromFileAsync(TopicsUpload upload);
}
