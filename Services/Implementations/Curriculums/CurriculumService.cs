using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Curriculums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Education.Api.Services.Implementations.Curriculums;

public class CurriculumService(ApplicationDbContext context) : ICurriculumService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<CurriculumDto> GetByIdAsync(int id)
    {
        return await _context
                .Curriculums
                .AsNoTracking()
                .Select(
                    c =>
                        new CurriculumDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            CreatedAt = c.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($@"Curriculum with ID ""{id}"" does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of curriculums.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{CurriculumDto}"/> containing the list of curriculums for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<CurriculumDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.Curriculums.OrderByDescending(c => c.CreatedAt).AsQueryable();

        List<CurriculumDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(
                c =>
                    new CurriculumDto
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

        return new PageInfo<CurriculumDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }
    //Updates a curriculum with a given ID
    public async Task UpdateAs

    //Deletes a curriculum with a given ID
    public async Task DeleteAsync(int id)
    {
        var curriculum =
            await _context.Curriculums.FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($@"Curriculum with ID ""{id}"" does not exist.");

        _context.Curriculums.Remove(curriculum);

        await _context.SaveChangesAsync();
    }
}
