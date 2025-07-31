using Education.Api.Data;
using Education.Api.Dtos.Topics;
using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Models.Topics;
using Education.Api.Services.Abstractions.Topics;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Topics;

public class SubtopicService(ApplicationDbContext context) : ISubtopicService
{
    private readonly ApplicationDbContext _context = context;

    //Gets a subtopic with a given ID
    public async Task<SubtopicDto> GetByIdAsync(int id)
    {
        return await _context
                .Subtopics
                .AsNoTracking()
                .Select(
                    st =>
                        new SubtopicDto
                        {
                            Id = st.Id,
                            Name = st.Name,
                            TopicId = st.TopicId,
                            CreatedAt = st.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(st => st.Id == id)
            ?? throw new KeyNotFoundException($"Subtopic with ID '{id}' does not exist");
    }

    /// <summary>
    /// Retrieves a paginated list of subtopics.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{SubtopicDto}"/> containing the list of subtopics for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<SubtopicDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.Subtopics.OrderByDescending(st => st.CreatedAt).AsQueryable();

        List<SubtopicDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(
                st =>
                    new SubtopicDto
                    {
                        Id = st.Id,
                        Name = st.Name,
                        TopicId = st.TopicId,
                        CreatedAt = st.CreatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<SubtopicDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Adds a new subtopic to the database.
    /// </summary>
    /// <param name="dto">The DTO containing the subtopic's data.</param>
    /// <returns>
    /// A <see cref="SubtopicDto"/> representing the newly created subtopic.
    /// </returns>
    /// <exception cref="ConflictException">
    /// Thrown if a subtopic with the same name already exists under the selected topic (case-insensitive).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more of the provided subject IDs do not exist.
    /// </exception>
    public async Task<SubtopicDto> AddAsync(AddSubtopicDto dto)
    {
        //get the selected topic for the subtopic
        var topic =
            await _context.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id.Equals(dto.TopicId))
            ?? throw new KeyNotFoundException($"Topic with ID '{dto.TopicId}' does not exist.");

        //Subtopic name is unique for each topic.
        //Check if there isn't already another subtopic with the given name under the selected topic
        bool alreadyExists = await _context
            .Subtopics
            .AnyAsync(st => st.Name.ToLower().Equals(dto.Name.ToLower()) && st.TopicId == topic.Id);

        if (alreadyExists)
        {
            throw new ConflictException(
                $"A subtopic with name '{dto.Name}' already exists under the topic '{topic.Name}'."
            );
        }

        //add the new subtopic to the database
        Subtopic subtopic = new() { Name = dto.Name, TopicId = topic.Id };

        await _context.Subtopics.AddAsync(subtopic);

        return SubtopicDto.MapFrom(subtopic);
    }

    /// <summary>
    /// Updates an existing subtopic with a given ID.
    /// </summary>
    /// <param name="id">The ID of the subtopic to update.</param>
    /// <param name="dto">The DTO containing the updated subtopic</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no subtopic with the specified ID exists.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if another subtopic with the same name already exists (case-insensitive).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more of the provided subject IDs do not exist.
    /// </exception>

    public async Task UpdateAsync(int id, UpdateSubtopicDto dto)
    {
        var subtopic =
            await _context.Subtopics.FirstOrDefaultAsync(t => t.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Subtopic with ID '{id}' does not exist.");

        //subtopic name is unique.
        //check if there isn't already an existing subtopic with the new updated name
        bool alreadyExists = await _context
            .Subtopics
            .AnyAsync(t => t.Name.ToLower().Equals(dto.Name.ToLower()) && t.Id != id);
        if (alreadyExists)
        {
            throw new ConflictException($"A subtopic with name '{dto.Name}' already exists.");
        }

        //get the the selected subjects for the subtopic
        var selectedSubjects = await _context
            .Subjects
            .Where(s => dto.SubjectIds.Contains(s.Id))
            .ToListAsync();

        //Make sure all the selected subjects exist
        if (selectedSubjects.Count != dto.SubjectIds.Count)
        {
            throw new InvalidOperationException("One or more selected subjects do not exist.");
        }

        subtopic.Name = dto.Name;
        subtopic.Subjects.AddRange(selectedSubjects);

        await _context.SaveChangesAsync();
    }

    //Deletes a subtopic with a given ID
    public async Task DeleteAsync(int id)
    {
        var subtopic =
            await _context.Subtopics.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Subtopic with ID '{id}' does not exist.");

        _context.Subtopics.Remove(subtopic);

        await _context.SaveChangesAsync();
    }
}
