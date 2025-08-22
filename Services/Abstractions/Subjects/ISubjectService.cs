using Education.Api.Dtos.Subjects;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Subjects;

public interface ISubjectService
{
    Task<SubjectDto> GetByIdAsync(int id);

    Task<PageInfo<SubjectDto>> GetAsync(SubjectQueryParams queryParams);

    Task AddAsync(AddSubjectDto dto);

    Task UpdateAsync(int id, UpdateSubjectDto dto);

    Task DeleteAsync(int id);
}
