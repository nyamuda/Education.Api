using Education.Api.Data;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Comments;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Comments;

public class QuestionCommentService(ApplicationDbContext context, ILogger<CommentService> logger)
    : CommentService(context, logger),
        IQuestionCommentService
{
    /// <summary>
    /// Retrieves a paginated list of comments for a specific question.
    /// </summary>
    /// <param name="questionId">The ID of the question to fetch comments for.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of comments per page.</param>
    /// <returns>A paginated list of comments associated with the question.</returns>
    public async Task<PageInfo<CommentDto>> GetAsync(int questionId, int page, int pageSize)
    {
        var query = _context
            .Comments
            .Where(c => c.QuestionId.Equals(questionId))
            .OrderByDescending(c => c.CreatedAt)
            .AsQueryable();

        List<CommentDto> comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;
        return new PageInfo<CommentDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = comments,
        };
    }

    /// <summary>
    /// Adds a new comment to the specified question.
    /// </summary>
    /// <param name="userId">The ID of the user adding the comment.</param>
    /// <param name="questionId">The ID of the question to comment on.</param>
    /// <param name="dto">The DTO containing the content of the comment.</param>
    /// <returns>The newly created comment.</returns>
    Task<CommentDto> AddAsync(int userId, int questionId, AddCommentDto dto);
}
