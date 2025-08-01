using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Questions;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Flags;

/// <summary>
/// Service interface for managing flags for questions.
/// </summary>
public interface IQuestionFlagService
{
    /// <summary>
    /// Retrieves a question flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the question flag to retrieve.</param>
    /// <returns>The question flag associated with the specified ID.</returns>
    Task<QuestionFlagDto> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a paginated list of flags for questions.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of question flags per page.</param>
    /// <returns>A paginated list of flag for questions.</returns>
    Task<PageInfo<QuestionFlagDto>> GetAsync(int page, int pageSize);

    /// <summary>
    /// Adds a new flag for the specified question.
    /// </summary>
    /// <param name="userId">The ID of the user adding the flag.</param>
    /// <param name="questionId">The ID of the question to flag on.</param>
    /// <param name="dto">The DTO containing the content of the flag.</param>
    /// <returns>The newly created flag for the question.</returns>
    Task<QuestionFlagDto> AddAsync(int userId, int questionId, AddQuestionFlagDto dto);

    /// <summary>
    /// Deletes a flag for a question.
    /// </summary>
    /// <param name="id">The ID of the question flag to delete.</param>
    Task DeleteAsync(int id);
}
