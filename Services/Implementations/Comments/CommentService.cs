using Education.Api.Data;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Upvotes;
using Education.Api.Dtos.Users;
using Education.Api.Services.Abstractions.Comments;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Comments;

public class CommentService(ApplicationDbContext context, ILogger<CommentService> logger)
    : ICommentService
{
    protected readonly ApplicationDbContext _context = context;
    protected readonly ILogger<CommentService> _logger = logger;

    /// <summary>
    /// Retrieves a comment by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the comment to retrieve.</param>
    /// <returns>The comment associated with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified comment is not found.
    /// </exception>
    public async Task<CommentDto> GetByIdAsync(int id)
    {
        return await _context
                .Comments
                .AsNoTracking()
                .Select(
                    c =>
                        new CommentDto
                        {
                            Id = c.Id,
                            Content = c.Content,
                            UserId = c.UserId,
                            User =
                                c.User != null
                                    ? new UserDto { Id = c.User.Id, Username = c.User.Username }
                                    : null,
                            Upvotes = c.Upvotes
                                .Select(
                                    upv =>
                                        new UpvoteDto
                                        {
                                            Id = upv.Id,
                                            UserId = upv.UserId,
                                            CommentId = upv.CommentId
                                        }
                                )
                                .ToList(),
                            QuestionId = c.QuestionId,
                            AnswerId = c.AnswerId,
                            CreatedAt = c.CreatedAt,
                            UpdatedAt = c.UpdatedAt
                        }
                )
                .FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Comment with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Updates the content of a comment, if the specified user is the owner.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to update the comment.</param>
    /// <param name="commentId">The ID of the comment to update.</param>
    /// <param name="dto">The DTO containing the updated content.</param>
    ///  <exception cref="KeyNotFoundException">
    /// Thrown if the specified comment is not found.
    /// </exception>
    ///  <exception cref="UnauthorizedAccessException">
    /// Thrown if the comment doesn't belong to the specified user.
    /// </exception>
    public async Task UpdateAsync(int userId, int commentId, UpdateCommentDto dto)
    {
        //check if the comment exists
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id.Equals(commentId));

        if (comment is null)
        {
            _logger.LogWarning(
                "Unable to update comment. Comment not found: {CommentId}",
                commentId
            );
            throw new KeyNotFoundException($"Comment with ID '{commentId}' was not found.");
        }

        //make sure the comment belongs to the specified user
        if (comment.UserId != userId)
        {
            _logger.LogWarning(
                "Cannot update comment. User {UserId} does not own the comment {CommentId}",
                userId,
                commentId
            );

            throw new UnauthorizedAccessException("You're not authorized to update this comment.");
        }
        //update the comment and persist the changes to the database
        comment.Content = dto.Content;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a comment, if the specified user is the owner.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to delete the comment.</param>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified comment is not found.
    /// </exception>
    ///  <exception cref="UnauthorizedAccessException">
    /// Thrown if the comment doesn't belong to the specified user.
    /// </exception>
    public async Task DeleteAsync(int userId, int commentId)
    {
        //check if the comment exists
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id.Equals(commentId));

        if (comment is null)
        {
            _logger.LogWarning(
                "Unable to delete comment. Comment not found: {CommentId}",
                commentId
            );
            throw new KeyNotFoundException($"Comment with ID '{commentId}' was not found.");
        }

        //make sure the comment belongs to the specified user
        if (comment.UserId != userId)
        {
            _logger.LogWarning(
                "Cannot delete comment. User {UserId} does not own the comment {CommentId}",
                userId,
                commentId
            );

            throw new UnauthorizedAccessException("You're not authorized to delete this comment.");
        }

        //remove the comment from the database
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully deleted comment: {CommentId}", commentId);
    }
}
