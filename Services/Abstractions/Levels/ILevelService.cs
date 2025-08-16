using Education.Api.Dtos.Levels;
using Education.Api.Enums.Levels;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Levels;

public interface ILevelService
{
    Task<LevelDto> GetByIdAsync(int id);

    Task<PageInfo<LevelDto>> GetAsync(
        int? examBoardId,
        int page,
        int pageSize,
        LevelSortOption sortBy
    );

    Task<LevelDto> AddAsync(int examBoardId, AddLevelDto dto);

    Task UpdateAsync(int examBoardId, int levelId, UpdateLevelDto dto);

    Task DeleteAsync(int id);
}
