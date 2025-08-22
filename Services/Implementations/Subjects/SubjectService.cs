using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Dtos.Subjects;
using Education.Api.Enums.Subjects;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Subjects;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Subjects;

public class SubjectService(ApplicationDbContext context) : ISubjectService
{
    private readonly ApplicationDbContext _context = context;

    //Gets a subject with a given ID
    public async Task<SubjectDto> GetByIdAsync(int id)
    {
        return await _context
                .Subjects
                .AsNoTracking()
                .AsSplitQuery()
                .Select(
                    s =>
                        new SubjectDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            LevelId = s.LevelId,
                            Level =
                                s.Level != null
                                    ? new LevelDto
                                    {
                                        Id = s.Level.Id,
                                        Name = s.Level.Name,
                                        ExamBoardId = s.Level.ExamBoardId,
                                        ExamBoard =
                                            s.Level.ExamBoard != null
                                                ? new ExamBoardDto
                                                {
                                                    Id = s.Level.ExamBoard.Id,
                                                    Name = s.Level.ExamBoard.Name,
                                                    CurriculumId = s.Level.ExamBoard.CurriculumId,
                                                    Curriculum =
                                                        s.Level.ExamBoard.Curriculum != null
                                                            ? new CurriculumDto
                                                            {
                                                                Id = s.Level
                                                                    .ExamBoard
                                                                    .Curriculum
                                                                    .Id,
                                                                Name = s.Level
                                                                    .ExamBoard
                                                                    .Curriculum
                                                                    .Name
                                                            }
                                                            : null
                                                }
                                                : null,
                                    }
                                    : null,
                            CreatedAt = s.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Subject with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of subjects.
    /// </summary>
    /// <param name="queryParams">
    /// An object containing query parameters to filter, sort, and paginate the subjects.
    /// </param>
    /// <returns>
    /// A <see cref="PageInfo{SubjectDto}"/> containing the list of subjects for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<SubjectDto>> GetAsync(SubjectQueryParams queryParams)
    {
        var query = _context.Subjects.AsQueryable();

        //apply the curriculum filter
        query =
            queryParams.CurriculumId != null
                ? query.Where(
                    s =>
                        s.Level != null
                        && s.Level.ExamBoard != null
                        && s.Level.ExamBoard.CurriculumId == queryParams.CurriculumId
                )
                : query;
        //apply the exam board filter
        query =
            queryParams.ExamBoardId != null
                ? query.Where(
                    s => s.Level != null && s.Level.ExamBoardId == queryParams.ExamBoardId
                )
                : query;
        //apply the level filter
        query =
            queryParams.LevelId != null
                ? query.Where(s => s.LevelId == queryParams.LevelId)
                : query;

        //sort the items
        query = queryParams.SortBy switch
        {
            SubjectSortOption.Name => query.OrderByDescending(s => s.Name),
            _ => query.OrderByDescending(s => s.CreatedAt),
        };

        List<SubjectDto> items = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .AsNoTracking()
            .AsSplitQuery()
            .Select(
                s =>
                    new SubjectDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        LevelId = s.LevelId,
                        Level =
                            s.Level != null
                                ? new LevelDto
                                {
                                    Id = s.Level.Id,
                                    Name = s.Level.Name,
                                    ExamBoardId = s.Level.ExamBoardId,
                                    ExamBoard =
                                        s.Level.ExamBoard != null
                                            ? new ExamBoardDto
                                            {
                                                Id = s.Level.ExamBoard.Id,
                                                Name = s.Level.ExamBoard.Name,
                                                CurriculumId = s.Level.ExamBoard.CurriculumId,
                                                Curriculum =
                                                    s.Level.ExamBoard.Curriculum != null
                                                        ? new CurriculumDto
                                                        {
                                                            Id = s.Level.ExamBoard.Curriculum.Id,
                                                            Name = s.Level.ExamBoard.Curriculum.Name
                                                        }
                                                        : null
                                            }
                                            : null,
                                }
                                : null,
                        CreatedAt = s.CreatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > queryParams.Page * queryParams.PageSize;

        return new PageInfo<SubjectDto>
        {
            Page = queryParams.Page,
            PageSize = queryParams.PageSize,
            TotalItems = totalItems,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Creates a new subject under a specific level.
    /// </summary>
    /// <param name="dto">
    /// The DTO containing the subject name and the ID of the level the subject should be added to.
    /// </param>
    /// <remarks>
    /// A subject name is unique within each level (case-insensitive comparison).
    /// </remarks>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified level is not found.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if another subject with the same name already exists under the selected level (case-insensitive).
    /// </exception>
    public async Task<SubjectDto> AddAsync(AddSubjectDto dto)
    {
        //check if the specified level exists
        var level =
            await _context
                .Levels
                .Include(l => l.Subjects)
                .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.Id.Equals(dto.LevelId))
            ?? throw new KeyNotFoundException(
                $"Subject creation failed: level with ID '{dto.LevelId}' does not exist."
            );

        //Subject name is unique under each educational level.
        //Check if there isn't already another subject with the given updated name under the specified level (case-insensitive)
        bool alreadyExists = level
            .Subjects
            .Any(s => s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            throw new ConflictException(
                $"Unable to add subject: a subject with name '{dto.Name}' already exists under the selected level."
            );
        }

        //save the subject to the database
        Subject subject = new() { Name = dto.Name, LevelId = dto.LevelId };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return SubjectDto.MapFrom(subject);
    }

    /// <summary>
    /// Updates an existing subject with a given ID.
    /// </summary>
    /// <param name="id">The ID of the subject to update.</param>
    /// <param name="dto">The DTO containing the updated subject details.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no subject or level with the specified ID exists.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if another subject with the same name already exists under the selected level (case-insensitive).
    /// </exception>
    public async Task UpdateAsync(int id, UpdateSubjectDto dto)
    {
        //check if there specified subject exists
        var subject =
            await _context.Subjects.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Subject update failed: subject with ID '{id}' does not exist."
            );

        //check if there specified level exists
        var level =
            await _context
                .Levels
                .Include(l => l.Subjects)
                .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.Id.Equals(dto.LevelId))
            ?? throw new KeyNotFoundException(
                $"Subject update failed: level with ID '{dto.LevelId}' does not exist."
            );

        //Subject name is unique under each educational level.
        //Check if there isn't already another subject with the given updated name under the specified level (case-insensitive)
        bool alreadyExists = level
            .Subjects
            .Any(s => s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) && s.Id != id);

        if (alreadyExists)
        {
            throw new ConflictException(
                $"Update failed: a subject with name '{dto.Name}' already exists under the selected level."
            );
        }

        //update subject
        subject.Name = dto.Name;
        subject.LevelId = dto.LevelId;

        await _context.SaveChangesAsync();
    }

    //Deletes a subject with a given ID
    public async Task DeleteAsync(int id)
    {
        var subject =
            await _context.Subjects.FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException(
                $"Delete failed: subject with ID '{id}' does not exist."
            );

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();

        //DeleteBehavior between Subject and Topic is set to NoAction.
        //Manually delete all topics related to this subject.
        await _context.Topics.Where(t => t.SubjectId.Equals(id)).ExecuteDeleteAsync();
    }
}
