using Education.Api.Data;
using Education.Api.Dtos.Topics;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Models.Topics;
using Education.Api.Services.Abstractions.Topics;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Topics;

public class TopicService(ApplicationDbContext context) : ITopicService
{
    private readonly ApplicationDbContext _context = context;

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
    /// Thrown if a topic with the same name already exists (case-insensitive).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more of the provided subject IDs do not exist.
    /// </exception>
    public async Task<TopicDto> AddAsync(AddTopicDto dto)
    {
        //Topic name is unique.
        //Check if there isn't already another topic with the given name (case-insensitive)
        bool alreadyExists = await _context
            .Topics
            .AnyAsync(t => t.Name.ToLower().Equals(dto.Name.ToLower()));

        if (alreadyExists)
        {
            throw new ConflictException($"Topic with name '{dto.Name}' already exists.");
        }

        //get the the selected subjects for the topic
        var selectedSubjects = await _context
            .Subjects
            .Where(s => dto.SubjectIds.Contains(s.Id))
            .ToListAsync();

        //Make sure all the selected subjects exist
        if (selectedSubjects.Count != dto.SubjectIds.Count)
        {
            throw new InvalidOperationException("One or more selected subjects do not exist.");
        }

        //add the new topic to the database
        Topic topic = new() { Name = dto.Name };
        topic.Subjects.AddRange(selectedSubjects);

        await _context.Topics.AddAsync(topic);

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
    /// Thrown if one or more of the provided exam board IDs do not exist.
    /// </exception>

    public async Task UpdateAsync(int id, UpdateTopicDto dto)
    {
        var topic =
            await _context.Topics.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Topic with ID '{id}' does not exist.");

        //topic name is unique.
        //check if there isn't already an existing topic with the new updated name
        bool alreadyExists = await _context
            .Topics
            .AnyAsync(s => s.Name.ToLower().Equals(dto.Name.ToLower()) && s.Id != id);
        if (alreadyExists)
        {
            throw new ConflictException($"A topic with name '{dto.Name}' already exists.");
        }

        //get the the selected exam boards for the topic
        var selectedExamBoards = await _context
            .ExamBoards
            .Where(eb => dto.ExamBoardIds.Contains(eb.Id))
            .ToListAsync();

        //Make sure all the selected exam boards exist
        if (selectedExamBoards.Count != dto.ExamBoardIds.Count)
        {
            throw new InvalidOperationException("One or more selected exam boards do not exist.");
        }

        topic.Name = dto.Name;
        topic.ExamBoards.AddRange(selectedExamBoards);

        await _context.SaveChangesAsync();
    }

    //Deletes a topic with a given ID
    public async Task DeleteAsync(int id)
    {
        var topic =
            await _context.Topics.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Topic with ID '{id}' does not exist.");

        _context.Topics.Remove(topic);

        await _context.SaveChangesAsync();
    }
}
