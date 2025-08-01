using Education.Api.Dtos.Comments;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Comments;

/// <summary>
/// Service interface for managing comments on answers.
/// </summary>
public interface IAnswerCommentService : ICommentService
{
    /// <summary>
    /// Retrieves a paginated list of comments for a specific answer.
    /// </summary>
    /// <param name="answerId">The ID of the answer to fetch comments for.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of comments per page.</param>
    /// <returns>A paginated list of comments associated with the answer.</returns>
    Task<PageInfo<CommentDto>> GetAsync(int answerId, int page, int pageSize);

    /// <summary>
    /// Adds a new comment to the specified answer.
    /// </summary>
    /// <param name="userId">The ID of the user adding the comment.</param>
    /// <param name="answerId">The ID of the answer to comment on.</param>
    /// <param name="dto">The DTO containing the content of the comment.</param>
    /// <returns>The newly created comment.</returns>
    Task<CommentDto> AddAsync(int userId, int answerId, AddCommentDto dto);
}
