using Education.Api.Data;
using Education.Api.Dtos.Topics;
using Education.Api.Services.Abstractions.Topics;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Topics;

public class TopicService(ApplicationDbContext context) : ITopicService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<TopicDto> GetByIdAsync(int id)
    {
        return await _context
                .Topics
                .AsNoTracking()
                .Select(
                    t =>
                        new TopicDto
                        {
                            Id = t.Id,
                            Name = t.Name,
                            CreatedAt = t.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Topic with ID '{id}' does not exist");
    }

    Task<PageInfo<TopicDto>> GetAsync(int page, int pageSize);

    Task<TopicDto> AddAsync(AddTopicDto dto);

    Task UpdateAsync(int id, UpdateTopicDto dto);

    Task DeleteAsync(int id);
}
