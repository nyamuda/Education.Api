using Education.Api.Data;
using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Questions;
using Education.Api.Dtos.Users;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Models.Flags;
using Education.Api.Services.Abstractions.Flags;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Flags;

/// <summary>
/// Service for managing flags for questions.
/// </summary>
public class QuestionFlagService(ApplicationDbContext context, ILogger<QuestionFlagService> logger)
    : IQuestionFlagService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<QuestionFlagService> _logger = logger;

    /// <summary>
    /// Retrieves a question flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the question flag to retrieve.</param>
    /// <returns>The question flag associated with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified question flag is not found.
    /// </exception>
    public async Task<QuestionFlagDto> GetByIdAsync(int id)
    {
        return await _context
                .QuestionFlags
                .AsNoTracking()
                .Select(
                    x =>
                        new QuestionFlagDto
                        {
                            Id = x.Id,
                            Content = x.Content,
                            UserId = x.UserId,
                            User =
                                x.User != null
                                    ? new UserDto { Id = x.User.Id, Username = x.User.Username, }
                                    : null,
                            QuestionId = x.QuestionId,
                            FlagType = x.FlagType,
                            Status = x.Status,
                            CreatedAt = x.CreatedAt,
                        }
                )
                .FirstOrDefaultAsync(x => x.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Question flag with ID '{id}' not found.");
    }

    /// <summary>
    /// Retrieves a paginated list of flags for questions.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of question flags per page.</param>
    /// <returns>A paginated list of flags for questions.</returns>
    public async Task<PageInfo<QuestionFlagDto>> GetAsync(int page, int pageSize)
    {
        var query = _context.QuestionFlags.OrderByDescending(c => c.CreatedAt).AsQueryable();

        List<QuestionFlagDto> questionFlags = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(
                x =>
                    new QuestionFlagDto
                    {
                        Id = x.Id,
                        Content = x.Content,
                        UserId = x.UserId,
                        User =
                            x.User != null
                                ? new UserDto { Id = x.User.Id, Username = x.User.Username, }
                                : null,
                        QuestionId = x.QuestionId,
                        FlagType = x.FlagType,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;
        return new PageInfo<QuestionFlagDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = questionFlags,
        };
    }

    /// <summary>
    /// Adds a new flag for the specified question.
    /// </summary>
    /// <param name="userId">The ID of the user adding the flag.</param>
    /// <param name="questionId">The ID of the question to flag on.</param>
    /// <param name="dto">The DTO containing the content of the flag.</param>
    /// <returns>The newly created flag for the question.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified user or question to flag on is not found.
    /// </exception>
    ///  <exception cref="ConflictException">
    /// Thrown if the specified user has already flagged the same question.
    /// </exception>
    public async Task<QuestionFlagDto> AddAsync(int userId, int questionId, AddQuestionFlagDto dto)
    {
        //check if there isn't already an existing flag for the same question by the same user
        bool hasAlreadyFlagged = await _context
            .QuestionFlags
            .Where(qf => qf.UserId.Equals(userId) && qf.QuestionId.Equals(questionId))
            .AnyAsync();

        if (hasAlreadyFlagged)
        {
            _logger.LogWarning(
                "Flag ignored: User '{UserId}' has already submitted a flag for question '{QuestionId}'.",
                userId,
                questionId
            );

            throw new ConflictException($"You’ve already flagged this question.");
        }

        //check if the question to flag on exists
        var question = await _context
            .Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(questionId));

        if (question is null)
        {
            _logger.LogWarning(
                "Unable to add flag. Question to flag not found: {QuestionId}.",
                questionId
            );
            throw new KeyNotFoundException(
                $"Question to flag with ID '{questionId}' does not exist."
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
                "Unable to add flag. User attempting to flag a question not found: {UserId}.",
                userId
            );
            throw new KeyNotFoundException(
                $"User with ID '{userId}' attempting to flag a question does not exist."
            );
        }

        //add the new question flag to the database
        QuestionFlag flag =
            new()
            {
                Content = dto.Content,
                UserId = userId,
                QuestionId = questionId,
                FlagType = dto.FlagType
            };

        await _context.QuestionFlags.AddAsync(flag);

        _logger.LogInformation("Added a new question flag by user: {UserId}", userId);

        return QuestionFlagDto.MapFrom(flag);
    }

    /// <summary>
    /// Deletes a flag for a question.
    /// </summary>
    /// <param name="id">The ID of the question flag to delete.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified question flag is not found.
    /// </exception>
    public async Task DeleteAsync(int id)
    {
        //check if the question flag exists
        var questionFlag = await _context.QuestionFlags.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (questionFlag is null)
        {
            _logger.LogWarning(
                "Unable to delete flag. Question flag not found: {QuestionFlagId}.",
                id
            );
            throw new KeyNotFoundException($"Question flag with ID '{id}' does not exist.");
        }

        //delete the question flag
        _context.QuestionFlags.Remove(questionFlag);

        await _context.SaveChangesAsync();
    }
}
