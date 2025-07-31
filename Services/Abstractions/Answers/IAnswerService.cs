using Education.Api.Dtos.Answers;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Answers;

public interface IAnswerService
{
    Task<AnswerDto> GetByIdAsync(int id);

    Task<PageInfo<AnswerDto>> GetAsync(int questionId, int page, int pageSize);

    Task<AnswerDto> AddAsync(int userId, AddAnswerDto dto);

    Task UpdateAsync(int userId, int answerId, UpdateAnswerDto dto);

    Task DeleteAsync(int userId, int answerId);
}
