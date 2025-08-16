using Education.Api.Dtos.ExamBoards;
using Education.Api.Enums.ExamBoards;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.ExamBoards;

public interface IExamBoardService
{
    Task<ExamBoardDto> GetByIdAsync(int id);

    Task<PageInfo<ExamBoardDto>> GetAsync(
        int page,
        int pageSize,
        ExamBoardSortOption sortBy,
        int? curriculumId
    );

    Task<ExamBoardDto> AddAsync(AddExamBoardDto dto);

    Task UpdateAsync(int id, UpdateExamBoardDto dto);

    Task DeleteAsync(int id);
}
