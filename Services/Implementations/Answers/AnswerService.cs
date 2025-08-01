using Education.Api.Data;
using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Answers;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Answers;

public class AnswerService(ApplicationDbContext context, ILogger<AnswerService> logger)
    : IAnswerService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<AnswerService> _logger = logger;

    //Gets an answer with a given ID
    public async Task<AnswerDto> GetByIdAsync(int id)
    {
        return await _context
                .Answers
                .AsNoTracking()
                .Select(
                    a =>
                        new AnswerDto
                        {
                            Id = a.Id,
                            Content = a.Content,
                            QuestionId = a.QuestionId,
                            UserId = a.UserId,
                            User =
                                a.User != null
                                    ? new UserDto { Id = a.User.Id, Username = a.User.Username, }
                                    : null,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt
                        }
                )
                .FirstOrDefaultAsync(a => a.Id.Equals(id))
            ?? throw new KeyNotFoundException($"Answer with ID '{id}' does not exist.");
    }

    /// <summary>
    /// Retrieves a paginated list of answers for a question with a given ID.
    /// </summary>
    /// <param name="questionId">The ID of the question whose answers are to be retrieved.</param>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The number of items to include per page.</param>
    /// <returns>
    /// A <see cref="PageInfo{AnswerDto}"/> containing the list of answers for the specified page,
    /// along with pagination metadata such as page number, page size, and whether more items are available.
    /// </returns>
    public async Task<PageInfo<AnswerDto>> GetAsync(int questionId, int page, int pageSize)
    {
        var query = _context
            .Answers
            .Where(a => a.QuestionId.Equals(questionId))
            .OrderByDescending(a => a.CreatedAt)
            .AsQueryable();

        List<AnswerDto> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .Select(
                a =>
                    new AnswerDto
                    {
                        Id = a.Id,
                        Content = a.Content,
                        QuestionId = a.QuestionId,
                        UserId = a.UserId,
                        User =
                            a.User != null
                                ? new UserDto { Id = a.User.Id, Username = a.User.Username, }
                                : null,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    }
            )
            .ToListAsync();

        //pagination info
        int totalItems = await query.CountAsync();
        bool hasMore = totalItems > page * pageSize;

        return new PageInfo<AnswerDto>
        {
            Page = page,
            PageSize = pageSize,
            HasMore = hasMore,
            Items = items
        };
    }

    /// <summary>
    /// Adds a new answer for a specified question to the database.
    /// </summary>
    ///  <param name="userId">The ID of the user adding the answer.</param>
    ///  <param name="questionId">The ID of the question being answered.</param>
    /// <param name="dto">The DTO containing the answer's content.</param>
    /// <returns>
    /// A <see cref="AnswerDto"/> representing the newly created answer.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified question does not exist.
    /// </exception>
    public async Task<AnswerDto> AddAsync(int userId, int questionId, AddAnswerDto dto)
    {
        //Check if the question being answered exists
        var question = await _context
            .Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id.Equals(questionId));

        if (question is null)
        {
            _logger.LogWarning(
                "Unable to add new answer.Cannot find the question: {QuestionId}.",
                questionId
            );
            throw new KeyNotFoundException($"Question with ID '{questionId}' does not exist.");
        }

        //add the new answer to the database
        Answer answer =
            new()
            {
                Content = dto.Content,
                QuestionId = questionId,
                UserId = userId
            };

        await _context.Answers.AddAsync(answer);

        _logger.LogInformation("Successfully added a new answer by user: {UserId}.", userId);

        return AnswerDto.MapFrom(answer);
    }

    /// <summary>
    /// Updates an existing answer.
    /// </summary>
    /// <param name="userId">The ID of the user attempting the update.</param>
    /// <param name="answerId">The ID of the soon to be updated answer.</param>
    /// <param name="dto">The DTO containing the answer's updated content.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the answer with the given ID cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the answer doesn't belong to the user attempting the update.
    /// </exception>

    public async Task UpdateAsync(int userId, int answerId, UpdateAnswerDto dto)
    {
        //Check if the answer exists
        var answer = await _context.Answers.FirstOrDefaultAsync(q => q.Id.Equals(answerId));

        if (answer is null)
        {
            _logger.LogWarning(
                "Unable to update answer. Cannot find the answer: {AnswerId}.",
                answerId
            );
            throw new KeyNotFoundException($"Answer with ID '{answerId}' does not exist.");
        }
        //Make sure the answer belongs to the specified user
        if (answer.UserId != userId)
        {
            _logger.LogWarning(
                "Cannot update answer. Answer does not belong to specified user: {UserId}",
                userId
            );
            throw new UnauthorizedAccessException("You're not authorized to update this answer");
        }

        //Persist the changes to the database
        answer.Content = dto.Content;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully updated answer: {AnswerId}", answerId);
    }

    /// <summary>
    ///  Deletes an existing answer with a given ID.
    /// </summary>
    /// <param name="userId">The ID of the user attempting to delete the answer.</param>
    /// <param name="answerId">The ID of the soon to be deleted answer.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the answer is not found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the answer does not belong to the specified user.
    /// </exception>
    public async Task DeleteAsync(int userId, int answerId)
    {
        //Check if the answer exists
        var answer = await _context.Answers.FirstOrDefaultAsync(q => q.Id.Equals(answerId));

        if (answer is null)
        {
            _logger.LogWarning(
                "Unable to delete answer. Cannot find the answer: {AnswerId}.",
                answerId
            );
            throw new KeyNotFoundException($"Answer with ID '{answerId}' does not exist.");
        }
        //Make sure the answer belongs to the specified user
        if (answer.UserId != userId)
        {
            _logger.LogWarning(
                "Cannot delete answer. Answer does not belong to specified user: {UserId}",
                userId
            );
            throw new UnauthorizedAccessException("You're not authorized to delete this answer.");
        }

        //Remove the answer from the database
        _context.Answers.Remove(answer);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully deleted answer: {AnswerId}", answerId);
    }
}
