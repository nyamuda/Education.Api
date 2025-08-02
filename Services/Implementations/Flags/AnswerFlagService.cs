using Education.Api.Data;
using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Answers;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Models.Flags;
using Education.Api.Services.Abstractions.Flags;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Flags;

/// <summary>
/// Service for managing flags for answers.
/// </summary>
public class AnswerFlagService(ApplicationDbContext context, ILogger<AnswerFlagService> logger)
    : IAnswerFlagService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<AnswerFlagService> _logger = logger;

    /// <summary>
    /// Retrieves an answer flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the answer flag to retrieve.</param>
    /// <returns>The answer flag associated with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified answer flag is not found.
    /// </exception>
    public async Task<AnswerFlagDto> GetByIdAsync(int id)
    {
        return await _context
                .AnswerFlags
                .AsNoTracking()
                .Select(
                    x =>
                        new AnswerFlagDto
                        {
                            Id = x.Id,
                            Content = x.Content,
                            UserId = x.UserId,
                            User =
                                x.User != null
                                    ? new UserDto { Id = x.User.Id, Username = x.User.Username, }
                                    : null,
                            AnswerId = x.AnswerId,
                            FlagType = x.FlagType,
                            Status = x.Status,
                            CreatedAt = x.CreatedAt,
                        }
                )
                .FirstOrDefaultAsync(x => x.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Answer flag with ID '{id}' not found.");
    }

    /// <summary>
    /// Retrieves a paginated list of flags for answers.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of answer flags per page.</param>
    /// <returns>A paginated list of flags for answers.</returns>
    public async Task<PageInfo<AnswerFlagDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.AnswerFlags.OrderByDescending(c => c.CreatedAt).AsQueryable();

        List<AnswerFlagDto> answerFlags = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(
                x =>
                    new AnswerFlagDto
                    {
                        Id = x.Id,
                        Content = x.Content,
                        UserId = x.UserId,
                        User =
                            x.User != null
                                ? new UserDto { Id = x.User.Id, Username = x.User.Username, }
                                : null,
                        AnswerId = x.AnswerId,
                        FlagType = x.FlagType,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;
        return new PageInfo<AnswerFlagDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = answerFlags,
        };
    }

    /// <summary>
    /// Adds a new flag for the specified answer.
    /// </summary>
    /// <param name="userId">The ID of the user adding the flag.</param>
    /// <param name="answerId">The ID of the answer to flag on.</param>
    /// <param name="dto">The DTO containing the content of the flag.</param>
    /// <returns>The newly created flag for the answer.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified user or answer to flag on is not found.
    /// </exception>
    public async Task<AnswerFlagDto> AddAsync(int userId, int answerId, AddAnswerFlagDto dto)
    {
        //check if the answer to flag on exists
        var answer = await _context
            .Answers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(answerId));

        if (answer is null)
        {
            _logger.LogWarning(
                "Unable to add flag. Answer to flag not found: {AnswerId}.",
                answerId
            );
            throw new KeyNotFoundException($"Answer to flag with ID '{answerId}' does not exist.");
        }
        //check if the user adding the flag exists
        var user = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id.Equals(userId));

        if (user is null)
        {
            _logger.LogWarning(
                "Unable to add flag. User attempting to flag an answer not found: {UserId}.",
                userId
            );
            throw new KeyNotFoundException(
                $"User with ID '{userId}' attempting to flag an answer does not exist."
            );
        }

        //add the new answer flag to the database
        AnswerFlag flag =
            new()
            {
                Content = dto.Content,
                UserId = userId,
                AnswerId = answerId,
                FlagType = dto.FlagType
            };

        await _context.AnswerFlags.AddAsync(flag);

        _logger.LogInformation("Added a new answer flag by user: {UserId}", userId);

        return AnswerFlagDto.MapFrom(flag);
    }

    /// <summary>
    /// Deletes a flag for an answer.
    /// </summary>
    /// <param name="id">The ID of the answer flag to delete.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified answer flag is not found.
    /// </exception>
    public async Task DeleteAsync(int id)
    {
        //check if the answer flag exists
        var answerFlag = await _context.AnswerFlags.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (answerFlag is null)
        {
            _logger.LogWarning("Unable to delete flag. Answer flag not found: {AnswerFlagId}.", id);

            throw new KeyNotFoundException($"Answer flag with ID '{id}' does not exist.");
        }

        //delete the answer flag
        _context.AnswerFlags.Remove(answerFlag);

        await _context.SaveChangesAsync();
    }
}
