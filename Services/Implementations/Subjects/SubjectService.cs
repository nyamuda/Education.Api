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
                    c =>
                        new SubjectDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            CreatedAt = c.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(c => c.Id.Equals(id))
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
        var query = _context.Subjects.OrderByDescending(c => c.CreatedAt).AsQueryable();

        List<SubjectDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(
                c =>
                    new SubjectDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        CreatedAt = c.CreatedAt
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
    /// Adds a new subject to the database after verifying that its name is unique.
    /// </summary>
    /// <param name="dto">The DTO containing the subject's data.</param>
    /// <returns>
    /// A <see cref="SubjectDto"/> representing the newly created subject.
    /// </returns>
    /// <exception cref="ConflictException">
    /// Thrown if a subject with the same name already exists (case-insensitive).
    /// </exception>

    public async Task<SubjectDto> AddAsync(AddSubjectDto dto)
    {
        //Subject name is unique.
        //Check if there isn't already another subject with the given name
        bool alreadyExists = await _context
            .Subjects
            .AnyAsync(c => c.Name.ToLower().Equals(dto.Name.ToLower()));

        if (alreadyExists)
        {
            throw new ConflictException($"Subject with name '{dto.Name}' already exists.");
        }

        //add the new subject to the database
        Subject subject = new() { Name = dto.Name };

        await _context.Subjects.AddAsync(subject);

        return SubjectDto.MapFrom(subject);
    }

    /// <summary>
    /// Updates an existing subject with a given ID.
    /// </summary>
    /// <param name="id">The ID of the subject to update.</param>
    /// <param name="dto">The DTO containing the updated subject</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if no subject with the specified ID exists.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if another subject with the same name already exists (case-insensitive).
    /// </exception>

    public async Task UpdateAsync(int id, UpdateSubjectDto dto)
    {
        var subject =
            await _context.Subjects.FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($@"Subject with ID ""{id}"" does not exist.");

        //subject name is unique.
        //check if there isn't already an existing subject with the new updated name
        bool alreadyExists = await _context
            .Subjects
            .AnyAsync(c => c.Name.ToLower().Equals(dto.Name.ToLower()) && c.Id != id);

        if (alreadyExists)
        {
            throw new ConflictException($"A subject with name '{dto.Name}' already exists.");
        }

        subject.Name = dto.Name;

        await _context.SaveChangesAsync();
    }

    //Deletes a subject with a given ID
    public async Task DeleteAsync(int id)
    {
        var subject =
            await _context.Subjects.FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($@"Subject with ID ""{id}"" does not exist.");

        _context.Subjects.Remove(subject);

        await _context.SaveChangesAsync();
    }
}
