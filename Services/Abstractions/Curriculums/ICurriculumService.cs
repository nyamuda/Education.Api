using Education.Api.Dtos.Curriculums;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Curriculums;

public interface ICurriculumService
{
    Task<CurriculumDto> GetByIdAsync(int id);

    Task<PageInfo<CurriculumDto>> GetAsync(int page, int pageSize);

    Task UpdateAsync(int id, UpdateCurriculumDto dto);

    Task DeleteAsync(int id);
}
