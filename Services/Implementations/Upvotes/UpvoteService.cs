using Education.Api.Data;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Upvotes;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Upvotes;

/// <summary>
/// Service for managing upvotes for questions, answers, and comments.
/// </summary>
public class UpvoteService(ApplicationDbContext context, ILogger<UpvoteService> logger)
    : IUpvoteService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<UpvoteService> _logger = logger;

    /// <summary>
    /// Adds an upvote to the specified question by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user upvoting the question.</param>
    /// <param name="questionId">The ID of the question to upvote.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified question or user is not found.
    /// </exception>
    public async Task UpvoteQuestionAsync(int userId, int questionId)
    {
        //check if the question exists
        var question = await _context
            .Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(questionId));

        if (question is null)
        {
            _logger.LogWarning(
                "Upvote failed: Question with ID '{QuestionId}' does not exist.",
                questionId
            );
            throw new KeyNotFoundException(
                $"Cannot upvote: No question found with ID '{questionId}'."
            );
        }

        //check if the user casting the upvote exists
        var user = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id.Equals(userId));

        if (user is null)
        {
            _logger.LogWarning("Upvote failed: User with ID '{UserId}' does not exist.", userId);

            throw new KeyNotFoundException($"Cannot upvote: No user found with ID '{userId}'.");
        }

        // upvote the question and the save the upvote to the database
        Upvote upvote = new() { UserId = userId, QuestionId = questionId };

        await _context.Upvotes.AddAsync(upvote);
    }

    Task RemoveQuestionUpvoteAsync(int userId, int questionId);

    Task UpvoteAnswerAsync(int userId, int answerId);

    Task RemoveAnswerUpvoteAsync(int userId, int answerId);

    Task UpvoteCommentAsync(int userId, int commentId);

    Task RemoveCommentUpvoteAsync(int userId, int commentId);
}
