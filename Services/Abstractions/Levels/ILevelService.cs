namespace Education.Api.Services.Abstractions.Levels;

public interface ILevelService 
{
    Task<CurriculumDto> GetByIdAsync(int id);

    Task<PageInfo<CurriculumDto>> GetAsync(int page, int pageSize);

    Task<CurriculumDto> AddAsync(AddCurriculumDto dto);

    Task UpdateAsync(int id, UpdateCurriculumDto dto);

    Task DeleteAsync(int id);
}