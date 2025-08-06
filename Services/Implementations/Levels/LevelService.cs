using Education.Api.Data;
using Education.Api.Dtos.Levels;
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
        return await _context.Levels.AsNoTracking().Select(l => new LevelDto
        {
            Id =l.Id,
            Name = l.Name,
            ExamBoardId = l.ExamBoardId,
            CreatedAt = l.CreatedAt
        }).FirstOrDefaultAsync(l => l.Id.Equals(id)) ??
        throw new KeyNotFoundException($"Level not found: ID '{id}'");
     }


    /// <summary>
    /// Retrieves a paginated list of levels for a given exam board.
    /// </summary>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{LevelDto}"/> containing the list of levels for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<LevelDto>> GetAsync(int examBoardId, int page, int pageSize) 
    {
        var query = _context.Levels.Where(l => l.ExamBoardId.Equals(examBoardId)).AsQueryable();

        List<LevelDto> items = await query
        .Skip((page = 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()
        .Select(l => new LevelDto
        {
            Id = l.Id,
            Name = l.Name,
            ExamBoardId = l.ExamBoardId,
            CreatedAt = l.CreatedAt
        }).ToListAsync();

        //pagination metadata
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<LevelDto>
        {
            Items = items,
            HasMore = hasMore,
            Page = page,
            PageSize = pageSize
        };
        
    }

    Task<LevelDto> AddAsync(AddLevelDto dto);

    Task UpdateAsync(int id, UpdateLevelDto dto);

    Task DeleteAsync(int id);
}
}
