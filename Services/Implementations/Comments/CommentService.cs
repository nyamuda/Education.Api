using Education.Api.Data;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Users;
using Education.Api.Services.Abstractions.Comments;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Comments;

public class CommentService(ApplicationDbContext context, ILogger<CommentService> logger)
    : ICommentService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CommentService> _logger = logger;

    /// <summary>
    /// Retrieves a comment by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the comment to retrieve.</param>
    /// <returns>The comment associated with the specified ID.</returns>
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
    ///
    Task UpdateAsync(int userId, int commentId, UpdateCommentDto dto);

    /// <summary>
    /// Deletes a comment, if the specified user is the owner.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to delete the comment.</param>
    /// <param name="commentId">The ID of the comment to delete.</param>
    Task DeleteAsync(int userId, int commentId);
}
