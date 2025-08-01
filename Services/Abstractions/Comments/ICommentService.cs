using Education.Api.Dtos.Comments;

namespace Education.Api.Services.Abstractions.Comments;

/// <summary>
/// Service interface for managing individual comment operations.
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Retrieves a comment by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the comment to retrieve.</param>
    /// <returns>The comment associated with the specified ID.</returns>
    Task<CommentDto> GetByIdAsync(int id);

    /// <summary>
    /// Updates the content of a comment, if the specified user is the owner.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to update the comment.</param>
    /// <param name="commentId">The ID of the comment to update.</param>
    /// <param name="dto">The DTO containing the updated content.</param>
    ///
    Task UpdateAsync(int userId, int commentId, UpdateCommentDto dto);

    /// <summary>
    /// Deletes a comment, if the specified user is the owner.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to delete the comment.</param>
    /// <param name="commentId">The ID of the comment to delete.</param>
    Task DeleteAsync(int userId, int commentId);
}
