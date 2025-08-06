using Education.Api.Dtos.Levels;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Levels;

public interface ILevelService
{
    Task<LevelDto> GetByIdAsync(int id);

    Task<PageInfo<LevelDto>> GetAsync(int page, int pageSize);

    Task<LevelDto> AddAsync(AddLevelDto dto);

    Task UpdateAsync(int id, UpdateLevelDto dto);

    Task DeleteAsync(int id);
}
