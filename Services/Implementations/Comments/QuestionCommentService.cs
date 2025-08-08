using Education.Api.Data;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Upvotes;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Comments;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Comments;

/// <summary>
/// Service for managing comments on questions.
/// </summary>
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
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified user or question to comment on is not found.
    /// </exception>
    public async Task<CommentDto> AddAsync(int userId, int questionId, AddCommentDto dto)
    {
        //check if the question to comment on exists
        var question = await _context
            .Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id.Equals(questionId));

        if (question is null)
        {
            _logger.LogWarning(
                "Unable to add comment. Question to comment on not found: {QuestionId}",
                questionId
            );
            throw new KeyNotFoundException(
                $"Question to comment on with ID '{questionId}' does not exist."
            );
        }
        //check if the user adding the comment exists
        var user = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id.Equals(userId));

        if (user is null)
        {
            _logger.LogWarning(
                "Unable to add comment. User attempting to comment on a question not found: {UserId}",
                userId
            );
            throw new KeyNotFoundException(
                $"User with ID '{userId}' attempting to comment on a question does not exist."
            );
        }

        //add the new comment to the database
        Comment comment =
            new()
            {
                Content = dto.Content,
                UserId = userId,
                QuestionId = questionId,
            };

        await _context.Comments.AddAsync(comment);

        _logger.LogInformation("Added a new question comment by user: {UserId}", userId);

        return CommentDto.MapFrom(comment);
    }
}
