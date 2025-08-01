using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Comments;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Flags;

/// <summary>
/// Service interface for managing flags for comments.
/// </summary>
public interface ICommentFlagService
{
    /// <summary>
    /// Retrieves a comment flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the comment flag to retrieve.</param>
    /// <returns>The comment flag associated with the specified ID.</returns>
    Task<CommentFlagDto> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a paginated list of comment flags.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of comment flags per page.</param>
    /// <returns>A paginated list of comment flags.</returns>
    Task<PageInfo<CommentFlagDto>> GetAsync(int page, int pageSize);

    /// <summary>
    /// Adds a new flag for the specified comment.
    /// </summary>
    /// <param name="userId">The ID of the user adding the flag.</param>
    /// <param name="commentId">The ID of the comment to flag on.</param>
    /// <param name="dto">The DTO containing the content of the flag.</param>
    /// <returns>The newly created flag for the comment.</returns>
    Task<CommentFlagDto> AddAsync(int userId, int commentId, AddCommentFlagDto dto);

    /// <summary>
    /// Deletes a flag for a comment.
    /// </summary>
    /// <param name="id">The ID of the comment flag to delete.</param>
    Task DeleteAsync(int id);
}
