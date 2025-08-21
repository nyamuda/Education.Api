using Education.Api.Data;
using Education.Api.Dtos.Subjects;
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
                .Select(
                    s =>
                        new SubjectDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            CreatedAt = s.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(s => s.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Subject with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of subjects.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{SubjectDto}"/> containing the list of subjects for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<SubjectDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.Subjects.OrderByDescending(s => s.CreatedAt).AsQueryable();

        List<SubjectDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(
                s =>
                    new SubjectDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        CreatedAt = s.CreatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<SubjectDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Adds a new subject to one or more educational levels.
    /// </summary>
    /// <param name="dto">
    /// The DTO containing the subject name and the IDs of the levels the subject should be added to.
    /// </param>
    /// <remarks>
    /// A subject name is unique within each level. If a subject with the same name already exists
    /// in a level (case-insensitive comparison), it will be skipped and not added again.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if one or more of the provided level IDs do not exist.
    /// </exception>
    public async Task AddAsync(AddSubjectDto dto)
    {
        //get the the selected educational levels for the subject
        var selectedLevels = await _context
            .Levels
            .Where(l => dto.LevelIds.Contains(l.Id))
            .Include(l => l.Subjects)
            .AsSplitQuery()
            .ToListAsync();

        //Make sure all the selected levels exist
        if (selectedLevels.Count != dto.LevelIds.Count)
        {
            throw new InvalidOperationException("One or more selected levels do not exist.");
        }

        //add the subject to each selected level
        foreach (var level in selectedLevels)
        {
            //Subject name is unique under each educational level.
            //Check if there isn't already another subject with the given name under the specified level (case-insensitive)
            bool alreadyExists = level
                .Subjects
                .Any(s => s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
                continue;

            level.Subjects.Add(new Subject { Name = dto.Name, LevelId = level.Id });
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing subject with a given ID.
    /// </summary>
    /// <param name="id">The ID of the subject to update.</param>
    /// <param name="dto">The DTO containing the updated subject</param>
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
                $"Update failed: subject with ID '{id}' does not exist."
            );

        //check if there specified level exists
        var _ =
            await _context.Levels.AsNoTracking().FirstOrDefaultAsync(l => l.Id.Equals(dto.LevelId))
            ?? throw new KeyNotFoundException(
                $"Update failed: level with ID '{dto.LevelId}' does not exist."
            );

        //Subject name is unique under each educational level.
        //Check if there isn't already another subject with the given updated name under the specified level (case-insensitive)
        bool alreadyExists = await _context
            .Subjects
            .AnyAsync(
                s =>
                    s.LevelId == dto.LevelId
                    && s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)
                    && s.Id != subject.Id
            );

        if (alreadyExists)
        {
            throw new ConflictException(
                $"Unable to update: a subject with name '{dto.Name}' already exists under the selected level."
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
