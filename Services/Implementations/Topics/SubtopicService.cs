using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Dtos.Subjects;
using Education.Api.Dtos.Topics;
using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Models.Topics;
using Education.Api.Services.Abstractions.Topics;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Topics;

public class SubtopicService(ApplicationDbContext context, ILogger<SubtopicService> logger)
    : ISubtopicService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<SubtopicService> _logger = logger;

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
                            Topic =
                                st.Topic != null
                                    ? new TopicDto
                                    {
                                        Id = st.Topic.Id,
                                        Name = st.Topic.Name,
                                        SubjectId = st.Topic.SubjectId,
                                        Subject =
                                            st.Topic.Subject != null
                                                ? new SubjectDto
                                                {
                                                    Id = st.Topic.Subject.Id,
                                                    Name = st.Topic.Subject.Name,
                                                    LevelId = st.Topic.Subject.LevelId,
                                                    Level =
                                                        st.Topic.Subject.Level != null
                                                            ? new LevelDto
                                                            {
                                                                Id = st.Topic.Subject.Level.Id,
                                                                Name = st.Topic.Subject.Level.Name,
                                                                ExamBoardId = st.Topic
                                                                    .Subject
                                                                    .Level
                                                                    .ExamBoardId,
                                                                ExamBoard =
                                                                    st.Topic.Subject.Level.ExamBoard
                                                                    != null
                                                                        ? new ExamBoardDto
                                                                        {
                                                                            Id = st.Topic
                                                                                .Subject
                                                                                .Level
                                                                                .ExamBoard
                                                                                .Id,
                                                                            Name = st.Topic
                                                                                .Subject
                                                                                .Level
                                                                                .ExamBoard
                                                                                .Name,
                                                                            CurriculumId = st.Topic
                                                                                .Subject
                                                                                .Level
                                                                                .ExamBoard
                                                                                .CurriculumId,
                                                                            Curriculum =
                                                                                st.Topic
                                                                                    .Subject
                                                                                    .Level
                                                                                    .ExamBoard
                                                                                    .Curriculum
                                                                                != null
                                                                                    ? new CurriculumDto
                                                                                    {
                                                                                        Id =
                                                                                            st.Topic
                                                                                                .Subject
                                                                                                .Level
                                                                                                .ExamBoard
                                                                                                .Curriculum!
                                                                                                .Id,
                                                                                        Name =
                                                                                            st.Topic
                                                                                                .Subject
                                                                                                .Level
                                                                                                .ExamBoard
                                                                                                .Curriculum!
                                                                                                .Name
                                                                                    }
                                                                                    : null
                                                                        }
                                                                        : null
                                                            }
                                                            : null,
                                                }
                                                : null,
                                    }
                                    : null,
                            CreatedAt = st.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(st => st.Id == id)
            ?? throw new KeyNotFoundException($"Subtopic with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of subtopics.
    /// </summary>
    /// <param name="queryParams">
    /// An object containing query parameters to filter, sort, and paginate the topics.
    /// </param>
    /// <returns>
    /// A <see cref="PageInfo{SubtopicDto}"/> containing the list of subtopics for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<SubtopicDto>> GetAsync(SubtopicQueryParams queryParams)
    {
        var query = _context.Subtopics.AsQueryable();

        //apply the curriculum filter
        query =
            queryParams.CurriculumId != null
                ? query.Where(
                    st =>
                        st.Topic != null
                        && st.Topic.Subject != null
                        && st.Topic.Subject.Level != null
                        && st.Topic.Subject.Level.ExamBoard != null
                        && st.Topic.Subject.Level.ExamBoard.CurriculumId == queryParams.CurriculumId
                )
                : query;
        //apply the exam board filter
        query =
            queryParams.ExamBoardId != null
                ? query.Where(
                    st =>
                        st.Topic != null
                        && st.Topic.Subject != null
                        && st.Topic.Subject.Level != null
                        && st.Topic.Subject.Level.ExamBoardId == queryParams.ExamBoardId
                )
                : query;
        //apply the level filter
        query =
            queryParams.LevelId != null
                ? query.Where(
                    st =>
                        st.Topic != null
                        && st.Topic.Subject != null
                        && st.Topic.Subject.LevelId == queryParams.LevelId
                )
                : query;

        //apply the subject filter
        query =
            queryParams.SubjectId != null
                ? query.Where(st => st.Topic != null && st.Topic.SubjectId == queryParams.SubjectId)
                : query;
        //sort the items
        query = queryParams.SortBy switch
        {
            TopicSortOption.Name => query.OrderByDescending(t => t.Name),
            _ => query.OrderByDescending(t => t.CreatedAt),
        };

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
                        Topic =
                            st.Topic != null
                                ? new TopicDto
                                {
                                    Id = st.Topic.Id,
                                    Name = st.Topic.Name,
                                    SubjectId = st.Topic.SubjectId,
                                    Subject =
                                        st.Topic.Subject != null
                                            ? new SubjectDto
                                            {
                                                Id = st.Topic.Subject.Id,
                                                Name = st.Topic.Subject.Name,
                                                LevelId = st.Topic.Subject.LevelId,
                                                Level =
                                                    st.Topic.Subject.Level != null
                                                        ? new LevelDto
                                                        {
                                                            Id = st.Topic.Subject.Level.Id,
                                                            Name = st.Topic.Subject.Level.Name,
                                                            ExamBoardId = st.Topic
                                                                .Subject
                                                                .Level
                                                                .ExamBoardId,
                                                            ExamBoard =
                                                                st.Topic.Subject.Level.ExamBoard
                                                                != null
                                                                    ? new ExamBoardDto
                                                                    {
                                                                        Id = st.Topic
                                                                            .Subject
                                                                            .Level
                                                                            .ExamBoard
                                                                            .Id,
                                                                        Name = st.Topic
                                                                            .Subject
                                                                            .Level
                                                                            .ExamBoard
                                                                            .Name,
                                                                        CurriculumId = st.Topic
                                                                            .Subject
                                                                            .Level
                                                                            .ExamBoard
                                                                            .CurriculumId,
                                                                        Curriculum =
                                                                            st.Topic
                                                                                .Subject
                                                                                .Level
                                                                                .ExamBoard
                                                                                .Curriculum != null
                                                                                ? new CurriculumDto
                                                                                {
                                                                                    Id = st.Topic
                                                                                        .Subject
                                                                                        .Level
                                                                                        .ExamBoard
                                                                                        .Curriculum!
                                                                                        .Id,
                                                                                    Name = st.Topic
                                                                                        .Subject
                                                                                        .Level
                                                                                        .ExamBoard
                                                                                        .Curriculum!
                                                                                        .Name
                                                                                }
                                                                                : null
                                                                    }
                                                                    : null
                                                        }
                                                        : null,
                                            }
                                            : null,
                                }
                                : null,
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
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the selected topic for the subtopic does not exist.
    /// </exception>
    public async Task<SubtopicDto> AddAsync(AddSubtopicDto dto)
    {
        //get the selected topic for the subtopic
        var topic =
            await _context.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id.Equals(dto.TopicId))
            ?? throw new KeyNotFoundException(
                $"Failed to add subtopic. Topic with ID '{dto.TopicId}' does not exist."
            );

        //Subtopic name is unique for each topic.
        //Check if there isn't already another subtopic with the given name under the selected topic.
        bool alreadyExists = await _context
            .Subtopics
            .AnyAsync(st => st.Name.ToLower().Equals(dto.Name.ToLower()) && st.TopicId == topic.Id);

        if (alreadyExists)
        {
            _logger.LogWarning(
                "A subtopic with name {SubtopicName} already exists under the topic {TopicName}.",
                dto.Name,
                topic.Name
            );

            throw new ConflictException(
                $"A subtopic with name '{dto.Name}' already exists under the topic '{topic.Name}'."
            );
        }

        //add the new subtopic to the database
        Subtopic subtopic = new() { Name = dto.Name, TopicId = dto.TopicId };

        await _context.Subtopics.AddAsync(subtopic);

        _logger.LogInformation("Subtopic created successfully: {SubtopicName}", dto.Name);

        return SubtopicDto.MapFrom(subtopic);
    }

    /// <summary>
    /// Updates an existing subtopic with a given ID.
    /// </summary>
    /// <param name="id">The ID of the subtopic to update.</param>
    /// <param name="dto">The DTO containing the updated subtopic</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the subtopic with the specified ID or selected topic for the subtopic does not exist.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if a subtopic with the same name already exists under the selected topic (case-insensitive).
    /// </exception>
    public async Task UpdateAsync(int id, UpdateSubtopicDto dto)
    {
        var subtopic =
            await _context.Subtopics.FirstOrDefaultAsync(t => t.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Update failed: subtopic with ID '{id}' does not exist."
            );

        //get the selected topic for the subtopic
        var topic =
            await _context.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id.Equals(dto.TopicId))
            ?? throw new KeyNotFoundException(
                $"Cannot update subtopic: topic with ID '{dto.TopicId}' does not exist."
            );

        //Subtopic name is unique for each topic.
        //Check if there isn't already another subtopic with the same updated name under the selected topic
        bool alreadyExists = await _context
            .Subtopics
            .AnyAsync(
                st =>
                    st.Name.ToLower().Equals(dto.Name.ToLower())
                    && st.Id != id
                    && st.TopicId == topic.Id
            );

        if (alreadyExists)
        {
            _logger.LogWarning(
                "A subtopic with name {SubtopicName} already exists under the topic {TopicName}.",
                dto.Name,
                topic.Name
            );

            throw new ConflictException(
                $"A subtopic with name '{dto.Name}' already exists under the topic '{topic.Name}'."
            );
        }

        subtopic.Name = dto.Name;
        subtopic.TopicId = dto.TopicId;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Subtopic updated successfully: {SubtopicId}", id);
    }

    //Deletes a subtopic with a given ID
    public async Task DeleteAsync(int id)
    {
        var subtopic =
            await _context.Subtopics.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Delete failed: subtopic with ID '{id}' does not exist."
            );

        _context.Subtopics.Remove(subtopic);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Subtopic deleted successfully: {SubtopicId}", id);
    }
}
