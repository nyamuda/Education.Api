using Education.Api.Data;
using Education.Api.Dtos.Topics;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Models.Topics;
using Education.Api.Services.Abstractions.Topics;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Topics;

public class TopicService(ApplicationDbContext context, ILogger<TopicService> logger)
    : ITopicService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<TopicService> _logger = logger;

    //Gets a topic with a given ID
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

    /// <summary>
    /// Retrieves a paginated list of topics.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{TopicDto}"/> containing the list of topics for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<TopicDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.Topics.OrderByDescending(t => t.CreatedAt).AsQueryable();

        List<TopicDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<TopicDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Adds a new topic to the database.
    /// </summary>
    /// <param name="dto">The DTO containing the topic's data.</param>
    /// <returns>
    /// A <see cref="TopicDto"/> representing the newly created topic.
    /// </returns>
    /// <exception cref="ConflictException">
    /// Thrown if a topic with the same name already exists under the specified subject (case-insensitive).
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified subject is not found.
    /// </exception>
    public async Task<TopicDto> AddAsync(AddTopicDto dto)
    {
        //check if the specified subject exists
        var subject =
            await _context.Subjects.FirstOrDefaultAsync(s => s.Id.Equals(dto.SubjectId))
            ?? throw new KeyNotFoundException(
                $"Unable to add topic: subject with ID ${dto.SubjectId} does not exist."
            );
        //Topic name is unique for each subject.
        //Check if there isn't already another topic with the given name under the specified subject (case-insensitive)
        bool alreadyExists = await _context
            .Topics
            .AnyAsync(
                t =>
                    t.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)
                    && t.SubjectId == dto.SubjectId
            );

        if (alreadyExists)
        {
            _logger.LogWarning(
                "Failed to add topic. Topic with name {TopicName} already exists under the selected subject.",
                dto.Name
            );
            throw new ConflictException(
                $"Unable to add topic. A topic with name '{dto.Name}' already exists under the selected subject."
            );
        }

        //add the new topic to the database
        Topic topic = new() { Name = dto.Name, SubjectId = dto.SubjectId };

        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New topic created with name {TopicName}", dto.Name);

        return TopicDto.MapFrom(topic);
    }

    /// <summary>
    /// Updates an existing topic with a given ID.
    /// </summary>
    /// <param name="id">The ID of the topic to update.</param>
    /// <param name="dto">The DTO containing the updated topic</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no topic with the specified ID exists.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if another topic with the same name already exists (case-insensitive).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more of the provided subject IDs do not exist.
    /// </exception>

    public async Task UpdateAsync(int id, UpdateTopicDto dto)
    {
        var topic =
            await _context.Topics.FirstOrDefaultAsync(t => t.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Cannot update topic. Topic with ID '{id}' does not exist."
            );

        //topic name is unique.
        //check if there isn't already an existing topic with the new updated name
        bool alreadyExists = await _context
            .Topics
            .AnyAsync(t => t.Name.ToLower().Equals(dto.Name.ToLower()) && t.Id != id);
        if (alreadyExists)
        {
            _logger.LogWarning(
                "Update failed: topic with name {TopicName} already exists.",
                dto.Name
            );

            throw new ConflictException(
                $"Update failed: a topic with name '{dto.Name}' already exists."
            );
        }

        //get the the selected subjects for the topic
        var selectedSubjects = await _context
            .Subjects
            .Where(s => dto.SubjectIds.Contains(s.Id))
            .ToListAsync();

        //Make sure all the selected subjects exist
        if (selectedSubjects.Count != dto.SubjectIds.Count)
        {
            _logger.LogWarning(
                "Update failed: one or more selected subjects do not exist for topic {TopicId}",
                id
            );
            throw new InvalidOperationException(
                "Topic update failed: one or more selected subjects do not exist."
            );
        }

        topic.Name = dto.Name;
        topic.Subjects.AddRange(selectedSubjects);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully updated topic: {TopicId}", id);
    }

    //Deletes a topic with a given ID
    public async Task DeleteAsync(int id)
    {
        var topic =
            await _context.Topics.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Delete failed: topic with ID '{id}' does not exist."
            );

        _context.Topics.Remove(topic);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully deleted topic: {TopicId}", id);
    }
}
