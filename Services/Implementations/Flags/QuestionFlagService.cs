using Education.Api.Data;
using Education.Api.Dtos.Flags.Questions;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Flags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Education.Api.Services.Implementations.Flags;

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
    /// <returns>A paginated list of flag for questions.</returns>
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
    Task<QuestionFlagDto> AddAsync(int userId, int questionId, AddQuestionFlagDto dto);

    /// <summary>
    /// Deletes a flag for a question.
    /// </summary>
    /// <param name="id">The ID of the question flag to delete.</param>
    Task DeleteAsync(int id);
}
