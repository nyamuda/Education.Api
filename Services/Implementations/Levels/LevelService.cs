using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Dtos.Subjects;
using Education.Api.Enums.Levels;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Levels;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Levels;

public class LevelService(ApplicationDbContext context, ILogger<LevelService> logger)
    : ILevelService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<LevelService> _logger = logger;

    //Gets a level with a given ID
    public async Task<LevelDto> GetByIdAsync(int id)
    {
        return await _context
                .Levels
                .AsNoTracking()
                .AsSplitQuery()
                .Select(
                    l =>
                        new LevelDto
                        {
                            Id = l.Id,
                            Name = l.Name,
                            ExamBoardId = l.ExamBoardId,
                            ExamBoard =
                                l.ExamBoard != null
                                    ? new ExamBoardDto
                                    {
                                        Id = l.ExamBoard.Id,
                                        Name = l.ExamBoard.Name,
                                        CurriculumId = l.ExamBoard.CurriculumId,
                                        Curriculum =
                                            l.ExamBoard.Curriculum != null
                                                ? new CurriculumDto
                                                {
                                                    Id = l.ExamBoard.Curriculum.Id,
                                                    Name = l.ExamBoard.Curriculum.Name,
                                                }
                                                : null,
                                    }
                                    : null,
                            Subjects = l.Subjects
                                .Select(s => new SubjectDto { Id = s.Id, Name = s.Name, })
                                .ToList(),
                            CreatedAt = l.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(l => l.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Level not found: ID '{id}'");
    }

    /// <summary>
    /// Retrieves a paginated list of levels for a given exam board.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <param name="examBoardId">The ID the exam board to filter the items by.</param>
    /// <param name="sortBy">The field to sort the items by.</param>
    /// <returns>
    /// A <see cref="PageInfo{LevelDto}"/> containing the list of levels for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<LevelDto>> GetAsync(
        int? curriculumId,
        int? examBoardId,
        int page,
        int pageSize,
        LevelSortOption sortBy
    )
    {
        var query = _context.Levels.AsQueryable();

        //apply filters
        if (curriculumId != null & curriculumId != 0)
        {
            query = query.Where(
                l => l.ExamBoard != null && l.ExamBoard.CurriculumId.Equals(curriculumId)
            );
        }

        if (examBoardId != null && examBoardId != 0)
        {
            query = query.Where(l => l.ExamBoardId.Equals(examBoardId));
        }

        //sort the items
        query = sortBy switch
        {
            LevelSortOption.Name => query.OrderByDescending(l => l.Name),
            _ => query.OrderByDescending(l => l.CreatedAt)
        };

        List<LevelDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .AsSplitQuery()
            .Select(
                l =>
                    new LevelDto
                    {
                        Id = l.Id,
                        Name = l.Name,
                        ExamBoardId = l.ExamBoardId,
                        ExamBoard =
                            l.ExamBoard != null
                                ? new ExamBoardDto
                                {
                                    Id = l.ExamBoard.Id,
                                    Name = l.ExamBoard.Name,
                                    CurriculumId = l.ExamBoard.CurriculumId,
                                    Curriculum =
                                        l.ExamBoard.Curriculum != null
                                            ? new CurriculumDto
                                            {
                                                Id = l.ExamBoard.Curriculum.Id,
                                                Name = l.ExamBoard.Curriculum.Name,
                                            }
                                            : null,
                                }
                                : null,
                        CreatedAt = l.CreatedAt
                    }
            )
            .ToListAsync();

        //pagination metadata
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<LevelDto>
        {
            Items = items,
            HasMore = hasMore,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Creates a new level for a specific exam board.
    /// </summary>
    /// <param name="examBoardId">The ID of the exam board the level is being added to.</param>
    /// <param name="dto">A DTO containing the level's details.</param>
    /// <returns>The newly created level.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified exam board is not found.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if there is already an existing level with the same name under the specified exam board.
    /// </exception>
    /// <remarks>
    /// The name of a level must be unique for each exam board.
    /// This means an exam board cannot have two or more levels with the same name.
    /// </remarks>
    public async Task<LevelDto> AddAsync(int examBoardId, AddLevelDto dto)
    {
        //check if the specified exam board exists
        var examBoard = await _context
            .ExamBoards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(examBoardId));
        if (examBoard is null)
        {
            _logger.LogWarning(
                "Failed to create new level: exam board {examBoardId} not found.",
                examBoardId
            );
            throw new KeyNotFoundException(
                $"Failed to create new level: exam board with ID '{examBoardId}' does not exist."
            );
        }
        //level name must be unique for exam board
        //check if there isn't already another level
        //with the same name under the specified exam board
        var hasLevelWithSameName = await _context
            .Levels
            .AnyAsync(l => l.Name.Equals(dto.Name) && l.ExamBoardId.Equals(examBoardId));
        if (hasLevelWithSameName)
        {
            _logger.LogWarning(
                "Level with the same name {levelName} already exists under exam board {examBoardId}.",
                dto.Name,
                examBoardId
            );

            throw new ConflictException(
                $"Level with the same name already exists under exam board with ID '{examBoardId}'."
            );
        }

        //create new level and save it to the database
        Level level = new() { Name = dto.Name, ExamBoardId = examBoardId };

        _context.Levels.Add(level);
        await _context.SaveChangesAsync();

        return LevelDto.MapFrom(level);
    }

    /// <summary>
    /// Updates a level with a given ID under a specific exam board.
    /// </summary>
    /// <param name="examBoardId">The ID of the exam board the level is under.</param>
    /// <param name="levelId">The ID of the soon to be updated level.</param>
    /// <param name="dto">A DTO containing the level's updated details.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified level or exam board is not found.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if there is already an existing level with the same name under the specified exam board.
    /// </exception>
    /// <remarks>
    /// The name of a level must be unique for each exam board.
    /// This means an exam board cannot have two or more levels with the same name.
    /// </remarks>
    public async Task UpdateAsync(int examBoardId, int levelId, UpdateLevelDto dto)
    {
        //check if the specified level exists
        var level = await _context.Levels.FirstOrDefaultAsync(l => l.Id.Equals(levelId));
        if (level is null)
        {
            _logger.LogWarning("Level update failed: level {levelId} not found.", levelId);
            throw new KeyNotFoundException(
                $"Level update failed: level with ID '{levelId}' does not exist."
            );
        }

        //check if the specified exam board exists
        var examBoard = await _context
            .ExamBoards
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(examBoardId));
        if (examBoard is null)
        {
            _logger.LogWarning(
                "Level update failed: exam board {examBoardId} not found.",
                examBoardId
            );
            throw new KeyNotFoundException(
                $"Level update failed: exam board with ID '{examBoardId}' does not exist."
            );
        }
        //level name must be unique for exam board
        //check if there isn't already another level
        //with the same name under the specified exam board
        var hasLevelWithSameName = await _context
            .Levels
            .AnyAsync(
                l => l.Name.Equals(dto.Name) && l.ExamBoardId.Equals(examBoardId) && l.Id != levelId
            );
        if (hasLevelWithSameName)
        {
            _logger.LogWarning(
                "Update failed: level with the same name {levelName} already exists under exam board {examBoardId}.",
                dto.Name,
                examBoardId
            );

            throw new ConflictException(
                $"Level with the same name already exists under exam board with ID '{examBoardId}'."
            );
        }

        //update level and persist changes to the database
        level.Name = dto.Name;
        level.ExamBoardId = examBoardId;

        await _context.SaveChangesAsync();
    }

    //Deletes a level with a given ID
    public async Task DeleteAsync(int id)
    {
        //check if the specified level exists
        var level = await _context.Levels.FirstOrDefaultAsync(l => l.Id.Equals(id));
        if (level is null)
        {
            _logger.LogWarning("Delete failed: level {levelId} not found.", id);
            throw new KeyNotFoundException($"Delete failed: level with ID '{id}' does not exist.");
        }

        _context.Levels.Remove(level);
        await _context.SaveChangesAsync();

        //DeleteBehavior between Level and Subject is set to NoAction.
        //Manually delete all subjects related to this level.
        await _context.Subjects.Where(s => s.LevelId.Equals(id)).ExecuteDeleteAsync();
    }
}
