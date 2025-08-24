using System.Text.Json;
using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Dtos.Subjects;
using Education.Api.Dtos.Topics;
using Education.Api.Enums.Topics;
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

    private readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNameCaseInsensitive = true };

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
                            SubjectId = t.SubjectId,
                            Subject =
                                t.Subject != null
                                    ? new SubjectDto
                                    {
                                        Id = t.Subject.Id,
                                        Name = t.Subject.Name,
                                        LevelId = t.Subject.LevelId,
                                        Level =
                                            t.Subject.Level != null
                                                ? new LevelDto
                                                {
                                                    Id = t.Subject.Level.Id,
                                                    Name = t.Subject.Level.Name,
                                                    ExamBoardId = t.Subject.Level.ExamBoardId,
                                                    ExamBoard =
                                                        t.Subject.Level.ExamBoard != null
                                                            ? new ExamBoardDto
                                                            {
                                                                Id = t.Subject.Level.ExamBoard.Id,
                                                                Name = t.Subject
                                                                    .Level
                                                                    .ExamBoard
                                                                    .Name,
                                                                CurriculumId = t.Subject
                                                                    .Level
                                                                    .ExamBoard
                                                                    .CurriculumId,
                                                                Curriculum =
                                                                    t.Subject
                                                                        .Level
                                                                        .ExamBoard
                                                                        .Curriculum != null
                                                                        ? new CurriculumDto
                                                                        {
                                                                            Id = t.Subject
                                                                                .Level
                                                                                .ExamBoard
                                                                                .Curriculum
                                                                                .Id,
                                                                            Name = t.Subject
                                                                                .Level
                                                                                .ExamBoard
                                                                                .Curriculum
                                                                                .Name
                                                                        }
                                                                        : null
                                                            }
                                                            : null
                                                }
                                                : null
                                    }
                                    : null,
                            Subto
                            CreatedAt = t.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Topic with ID '{id}' does not exist");
    }

    /// <summary>
    /// Retrieves a paginated list of topics.
    /// </summary>
    /// <param name="queryParams">
    /// An object containing query parameters to filter, sort, and paginate the topics.
    /// </param>
    /// <returns>
    /// A <see cref="PageInfo{TopicDto}"/> containing the list of topics for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<TopicDto>> GetAsync(TopicQueryParams queryParams)
    {
        var query = _context.Topics.AsQueryable();

        //apply the curriculum filter
        query =
            queryParams.CurriculumId != null
                ? query.Where(
                    t =>
                        t.Subject != null
                        && t.Subject.Level != null
                        && t.Subject.Level.ExamBoard != null
                        && t.Subject.Level.ExamBoard.CurriculumId == queryParams.CurriculumId
                )
                : query;
        //apply the exam board filter
        query =
            queryParams.ExamBoardId != null
                ? query.Where(
                    t =>
                        t.Subject != null
                        && t.Subject.Level != null
                        && t.Subject.Level.ExamBoardId == queryParams.ExamBoardId
                )
                : query;
        //apply the level filter
        query =
            queryParams.LevelId != null
                ? query.Where(t => t.Subject != null && t.Subject.LevelId == queryParams.LevelId)
                : query;

        //apply the subject filter
        query =
            queryParams.SubjectId != null
                ? query.Where(t => t.SubjectId == queryParams.SubjectId)
                : query;

        //sort the items
        query = queryParams.SortBy switch
        {
            TopicSortOption.Name => query.OrderByDescending(t => t.Name),
            _ => query.OrderByDescending(t => t.CreatedAt),
        };

        List<TopicDto> items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .AsSplitQuery()
            .AsNoTracking()
            .Select(
                t =>
                    new TopicDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        SubjectId = t.SubjectId,
                        Subject =
                            t.Subject != null
                                ? new SubjectDto
                                {
                                    Id = t.Subject.Id,
                                    Name = t.Subject.Name,
                                    LevelId = t.Subject.LevelId,
                                    Level =
                                        t.Subject.Level != null
                                            ? new LevelDto
                                            {
                                                Id = t.Subject.Level.Id,
                                                Name = t.Subject.Level.Name,
                                                ExamBoardId = t.Subject.Level.ExamBoardId,
                                                ExamBoard =
                                                    t.Subject.Level.ExamBoard != null
                                                        ? new ExamBoardDto
                                                        {
                                                            Id = t.Subject.Level.ExamBoard.Id,
                                                            Name = t.Subject.Level.ExamBoard.Name,
                                                            CurriculumId = t.Subject
                                                                .Level
                                                                .ExamBoard
                                                                .CurriculumId,
                                                            Curriculum =
                                                                t.Subject.Level.ExamBoard.Curriculum
                                                                != null
                                                                    ? new CurriculumDto
                                                                    {
                                                                        Id = t.Subject
                                                                            .Level
                                                                            .ExamBoard
                                                                            .Curriculum
                                                                            .Id,
                                                                        Name = t.Subject
                                                                            .Level
                                                                            .ExamBoard
                                                                            .Curriculum
                                                                            .Name
                                                                    }
                                                                    : null
                                                        }
                                                        : null
                                            }
                                            : null
                                }
                                : null,
                        CreatedAt = t.CreatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > queryParams.Page * queryParams.PageSize;

        return new PageInfo<TopicDto>
        {
            Page = queryParams.Page,
            PageSize = queryParams.PageSize,
            TotalItems = totalItems,
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
                t => t.Name.ToLower().Equals(dto.Name.ToLower()) && t.SubjectId == dto.SubjectId
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
    /// Adds multiple topics to the specified subject in bulk.
    /// Existing topics with the same name (case-insensitive) are skipped to avoid duplicates.
    /// Typically used when importing topics from a deserialized JSON file.
    /// </summary>
    /// <param name="subjectId">The ID of the subject to which the topics will be added.</param>
    /// <param name="topics">The list of topics to add.</param>
    public async Task AddTopicsToSubjectAsync(int subjectId, List<Topic> topics)
    {
        var subject =
            await _context
                .Subjects
                .Include(s => s.Topics)
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.Id == subjectId)
            ?? throw new KeyNotFoundException($"Subject with ID '{subjectId}' not found.");

        //check if there is already a topic with the given name
        foreach (Topic topic in topics)
        {
            bool alreadyExists = subject
                .Topics
                .Any(t => t.Name.Equals(topic.Name, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
                continue;

            subject.Topics.Add(topic);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing topic with a given ID.
    /// </summary>
    /// <param name="id">The ID of the topic to update.</param>
    /// <param name="dto">The DTO containing the updated topic</param>
    /// <exception cref="ConflictException">
    /// Thrown if another topic with the same name already exists under the specified subject (case-insensitive).
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    ///  Thrown if the specified topic or subject is not found.
    /// </exception>

    public async Task UpdateAsync(int id, UpdateTopicDto dto)
    {
        var topic =
            await _context.Topics.FirstOrDefaultAsync(t => t.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Unable to update topic. Topic with ID '{id}' does not exist."
            );

        //check if the specified subject exists
        var subject =
            await _context.Subjects.FirstOrDefaultAsync(s => s.Id.Equals(dto.SubjectId))
            ?? throw new KeyNotFoundException(
                $"Unable to update topic: subject with ID ${dto.SubjectId} does not exist."
            );

        //Topic name is unique for each subject.
        //Check if there isn't already another topic with the given name under the specified subject (case-insensitive)
        bool alreadyExists = await _context
            .Topics
            .AnyAsync(
                t =>
                    t.Name.ToLower().Equals(dto.Name.ToLower())
                    && t.SubjectId == dto.SubjectId
                    && t.Id != id
            );
        if (alreadyExists)
        {
            _logger.LogWarning(
                "Update failed: topic with name {TopicName} already exists under the selected subject.",
                dto.Name
            );

            throw new ConflictException(
                $"Update failed: a topic with name '{dto.Name}' already exists under the selected subject."
            );
        }

        topic.Name = dto.Name;
        topic.SubjectId = dto.SubjectId;

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

    /// <summary>
    /// Deserializes topics from a JSON a file.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the file size exceeds the maximum allowed size.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the JSON content cannot be deserialized into the expected list of topics.
    /// </exception>
    public async Task<List<Topic>> DeserializeTopicsFromFileAsync(TopicsUpload upload)
    {
        double maxFileSize = 5 * 1024 * 1024; //max size is (5MB)

        if (upload.File.Length > maxFileSize)
        {
            throw new InvalidOperationException(
                "Failed to add topics: file size cannot exceed 5MB."
            );
        }
        //read the json file
        using Stream stream = upload.File.OpenReadStream();

        //finally, deserialize the JSON
        var topics =
            await JsonSerializer.DeserializeAsync<List<Topic>>(stream, _jsonOptions)
            ?? throw new Exception(
                $"Topics deserialization failed: unable to deserialize the '{upload.File.Name}' file."
            );

        return topics;
    }
}
