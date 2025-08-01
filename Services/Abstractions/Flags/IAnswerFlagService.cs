using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Answers;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Flags;

/// <summary>
/// Service interface for managing flags for answers.
/// </summary>
public interface IAnswerFlagService
{
    /// <summary>
    /// Retrieves an answer flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the answer flag to retrieve.</param>
    /// <returns>The answer flag associated with the specified ID.</returns>
    Task<AnswerFlagDto> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a paginated list of flags for answers.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of answer flags per page.</param>
    /// <returns>A paginated list of flags for answers.</returns>
    Task<PageInfo<AnswerFlagDto>> GetAsync(int page, int pageSize);

    /// <summary>
    /// Adds a new flag for the specified answer.
    /// </summary>
    /// <param name="userId">The ID of the user adding the flag.</param>
    /// <param name="answerId">The ID of the answer to flag on.</param>
    /// <param name="dto">The DTO containing the content of the flag.</param>
    /// <returns>The newly created flag for the answer.</returns>
    Task<AnswerFlagDto> AddAsync(int userId, int answerId, AddAnswerFlagDto dto);

    /// <summary>
    /// Deletes a flag for an answer.
    /// </summary>
    /// <param name="id">The ID of the answer flag to delete.</param>
    Task DeleteAsync(int id);
}
