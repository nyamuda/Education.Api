using Education.Api.Dtos.Questions;
using Education.Api.Enums.Questions;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Questions;

public interface IQuestionService
{
    Task<QuestionDto> GetByIdAsync(int id);

    Task<PageInfo<QuestionDto>> GetAsync(int page, int pageSize);

    Task<QuestionDto> AddAsync(int userId, AddQuestionDto dto);

    Task UpdateAsync(int userId, int questionId, UpdateQuestionDto dto);

    Task UpdateStatusAsync(int userId, int questionId, QuestionStatus status);

    Task DeleteAsync(int userId, int questionId);
}
