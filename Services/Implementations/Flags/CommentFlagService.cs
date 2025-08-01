using Education.Api.Data;
using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Comments;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Models.Flags;
using Education.Api.Services.Abstractions.Flags;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Flags;

/// <summary>
/// Service for managing flags for comments.
/// </summary>
public class CommentFlagService(ApplicationDbContext context, ILogger<CommentFlagService> logger)
    : ICommentFlagService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CommentFlagService> _logger = logger;

    /// <summary>
    /// Retrieves a comment flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the comment flag to retrieve.</param>
    /// <returns>The comment flag associated with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified comment flag is not found.
    /// </exception>
    public async Task<CommentFlagDto> GetByIdAsync(int id)
    {
        return await _context
                .CommentFlags
                .AsNoTracking()
                .Select(
                    x =>
                        new CommentFlagDto
                        {
                            Id = x.Id,
                            Content = x.Content,
                            UserId = x.UserId,
                            User =
                                x.User != null
                                    ? new UserDto { Id = x.User.Id, Username = x.User.Username, }
                                    : null,
                            CommentId = x.CommentId,
                            Status = x.Status,
                            CreatedAt = x.CreatedAt,
                        }
                )
                .FirstOrDefaultAsync(x => x.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Comment flag with ID '{id}' not found.");
    }

    /// <summary>
    /// Retrieves a paginated list of flags for comments.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of comment flags per page.</param>
    /// <returns>A paginated list of flags for comments.</returns>
    public async Task<PageInfo<CommentFlagDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.CommentFlags.OrderByDescending(c => c.CreatedAt).AsQueryable();

        List<CommentFlagDto> commentFlags = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(
                x =>
                    new CommentFlagDto
                    {
                        Id = x.Id,
                        Content = x.Content,
                        UserId = x.UserId,
                        User =
                            x.User != null
                                ? new UserDto { Id = x.User.Id, Username = x.User.Username, }
                                : null,
                        CommentId = x.CommentId,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;
        return new PageInfo<CommentFlagDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = commentFlags,
        };
    }

    /// <summary>
    /// Adds a new flag for the specified comment.
    /// </summary>
    /// <param name="userId">The ID of the user adding the flag.</param>
    /// <param name="commentId">The ID of the comment to flag on.</param>
    /// <param name="dto">The DTO containing the content of the flag.</param>
    /// <returns>The newly created flag for the comment.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified user or comment to flag on is not found.
    /// </exception>
    public async Task<CommentFlagDto> AddAsync(int userId, int commentId, AddCommentFlagDto dto)
    {
        //check if the comment to flag on exists
        var comment = await _context
            .Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(commentId));

        if (comment is null)
        {
            _logger.LogWarning(
                "Unable to add flag. Comment to flag not found: {CommentId}.",
                commentId
            );
            throw new KeyNotFoundException(
                $"Comment to flag with ID '{commentId}' does not exist."
            );
        }
        //check if the user adding the flag exists
        var user = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id.Equals(userId));

        if (user is null)
        {
            _logger.LogWarning(
                "Unable to add flag. User attempting to flag a comment not found: {UserId}.",
                userId
            );
            throw new KeyNotFoundException(
                $"User with ID '{userId}' attempting to flag a comment does not exist."
            );
        }

        //add the new comment flag to the database
        CommentFlag flag =
            new()
            {
                Content = dto.Content,
                UserId = userId,
                CommentId = commentId,
                FlagType = dto.FlagType
            };

        await _context.CommentFlags.AddAsync(flag);

        _logger.LogInformation("Added a new comment flag by user: {UserId}", userId);

        return CommentFlagDto.MapFrom(flag);
    }

    /// <summary>
    /// Deletes a flag for a comment.
    /// </summary>
    /// <param name="id">The ID of the comment flag to delete.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified comment flag is not found.
    /// </exception>
    public async Task DeleteAsync(int id)
    {
        //check if the comment flag exists
        var commentFlag = await _context.CommentFlags.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (commentFlag is null)
        {
            _logger.LogWarning(
                "Unable to delete flag. Comment flag not found: {CommentFlagId}.",
                id
            );
            throw new KeyNotFoundException($"Comment flag with ID '{id}' does not exist.");
        }

        //delete the comment flag
        _context.CommentFlags.Remove(commentFlag);

        await _context.SaveChangesAsync();
    }
}
