using Education.Api.Data;
using Education.Api.Dtos.Levels;
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
            Id = id,
            Name = l.Name,
            ExamBoardId = l.ExamBoardId,
            CreatedAt = l.CreatedAt
        }).FirstOrDefaultAsync(l => l.Id.Equals(id)) ??
        throw new KeyNotFoundException($"Level not found: ID '{id}'");
     }

    Task<PageInfo<LevelDto>> GetAsync(int examBoardId, int page, int pageSize);

    Task<LevelDto> AddAsync(AddLevelDto dto);

    Task UpdateAsync(int id, UpdateLevelDto dto);

    Task DeleteAsync(int id);
}
}
