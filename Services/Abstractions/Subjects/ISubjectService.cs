using Education.Api.Dtos.Subjects;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Subjects;

public interface ISubjectService
{
    Task<SubjectDto> GetByIdAsync(int id);

    Task<PageInfo<SubjectDto>> GetAsync(SubjectQueryParams queryParams);

    Task<PageInfo<SubjectDto>> GetForLevelAsync(SubjectQueryParams queryParams);

    Task<SubjectDto> AddAsync(AddSubjectDto dto);

    Task UpdateAsync(int id, UpdateSubjectDto dto);

    Task DeleteAsync(int id);
}
