using Education.Api.Data;
using Education.Api.Exceptions;
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
    /// <exception cref="ConflictException">
    /// Thrown if the specified user has already upvoted the same question.
    /// </exception>
    public async Task UpvoteQuestionAsync(int userId, int questionId)
    {
        //check if there isn't already an existing upvote for the same question by the same user
        bool hasAlreadyUpvoted = await _context
            .Upvotes
            .Where(upv => upv.UserId.Equals(userId) && upv.QuestionId.Equals(questionId))
            .AnyAsync();

        if (hasAlreadyUpvoted)
        {
            _logger.LogWarning(
                "Upvote skipped: User {UserId} has already upvoted question {QuestionId}.",
                userId,
                questionId
            );
            throw new ConflictException(
                $"Duplicate upvote: User with ID '{userId}' has already upvoted question with ID '{questionId}'."
            );
        }

        //check if the question exists
        var question = await _context
            .Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(questionId));

        if (question is null)
        {
            _logger.LogWarning(
                "Upvote failed: Question with ID {QuestionId} does not exist.",
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
            _logger.LogWarning(
                "Question upvote failed: User with ID {UserId} does not exist.",
                userId
            );

            throw new KeyNotFoundException(
                $"Cannot upvote question: No user found with ID '{userId}'."
            );
        }

        // upvote the question and the save the upvote to the database
        Upvote upvote = new() { UserId = userId, QuestionId = questionId };

        await _context.Upvotes.AddAsync(upvote);

        _logger.LogInformation("Successfully upvoted question with ID {QuestionId}.", questionId);
    }

    /// <summary>
    /// Removes an existing upvote from the specified question by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user removing their upvote.</param>
    /// <param name="questionId">The ID of the question to remove the upvote from.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified upvote is not not found.
    /// </exception>
    public async Task RemoveQuestionUpvoteAsync(int userId, int questionId)
    {
        //check if the question upvote exists
        var questionUpvote = await _context
            .Upvotes
            .FirstOrDefaultAsync(
                upv => upv.UserId.Equals(userId) && upv.QuestionId.Equals(questionId)
            );

        if (questionUpvote is null)
        {
            _logger.LogWarning(
                "Failed to remove upvote: Upvote for question with ID {QuestionId} by user with ID {UserId} not found.",
                questionId,
                userId
            );

            throw new KeyNotFoundException(
                $"Cannot remove upvote: Upvote for question with ID '{questionId}' by user with ID '{userId}' does not exist."
            );
        }

        //remove the upvote
        _context.Upvotes.Remove(questionUpvote);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Successfully removed an upvote for a question with ID {QuestionId}.",
            questionId
        );
    }

    /// <summary>
    /// Adds an upvote to the specified answer by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user upvoting the answer.</param>
    /// <param name="answerId">The ID of the answer to upvote.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified answer or user is not found.
    /// </exception>
    /// <exception cref="ConflictException">
    /// Thrown if the specified user has already upvoted the same answer.
    /// </exception>
    public async Task UpvoteAnswerAsync(int userId, int answerId)
    {
        //check if there isn't already an existing upvote for the same answer by the same user
        bool hasAlreadyUpvoted = await _context
            .Upvotes
            .Where(upv => upv.UserId.Equals(userId) && upv.AnswerId.Equals(answerId))
            .AnyAsync();

        if (hasAlreadyUpvoted)
        {
            _logger.LogWarning(
                "Upvote skipped: User {UserId} has already upvoted answer {AnswerId}.",
                userId,
                answerId
            );
            throw new ConflictException(
                $"Duplicate upvote: User with ID '{userId}' has already upvoted answer with ID '{answerId}'."
            );
        }

        //check if the answer exists
        var answer = await _context
            .Answers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(answerId));

        if (answer is null)
        {
            _logger.LogWarning(
                "Upvote failed: Answer with ID {AnswerId} does not exist.",
                answerId
            );
            throw new KeyNotFoundException($"Cannot upvote: No answer found with ID '{answerId}'.");
        }

        //check if the user casting the upvote exists
        var user = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id.Equals(userId));

        if (user is null)
        {
            _logger.LogWarning(
                "Answer upvote failed: User with ID {UserId} does not exist.",
                userId
            );

            throw new KeyNotFoundException(
                $"Cannot upvote answer: No user found with ID '{userId}'."
            );
        }

        // upvote the answer and the save the upvote to the database
        Upvote upvote = new() { UserId = userId, AnswerId = answerId };

        await _context.Upvotes.AddAsync(upvote);

        _logger.LogInformation("Successfully upvoted answer with ID {AnswerId}.", answerId);
    }

    /// <summary>
    /// Removes an existing upvote from the specified answer by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user removing their upvote.</param>
    /// <param name="answerId">The ID of the answer to remove the upvote from.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified upvote is not not found.
    /// </exception>
    public async Task RemoveAnswerUpvoteAsync(int userId, int answerId)
    {
        //check if the answer upvote exists
        var answerUpvote = await _context
            .Upvotes
            .FirstOrDefaultAsync(upv => upv.UserId.Equals(userId) && upv.AnswerId.Equals(answerId));

        if (answerUpvote is null)
        {
            _logger.LogWarning(
                "Failed to remove upvote: Upvote for answer with ID {AnswerId} by user with ID {UserId} not found.",
                answerId,
                userId
            );

            throw new KeyNotFoundException(
                $"Cannot remove upvote: Upvote for answer with ID '{answerId}' by user with ID '{userId}' does not exist."
            );
        }

        //remove the upvote
        _context.Upvotes.Remove(answerUpvote);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Successfully removed an upvote for an answer with ID {AnswerId}.",
            answerId
        );
    }

    /// <summary>
    /// Adds an upvote to the specified comment by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user upvoting the comment.</param>
    /// <param name="commentId">The ID of the comment to upvote.</param>
    Task UpvoteCommentAsync(int userId, int commentId);

    /// <summary>
    /// Removes an existing upvote from the specified comment by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user removing their upvote.</param>
    /// <param name="commentId">The ID of the comment to remove the upvote from.</param>
    Task RemoveCommentUpvoteAsync(int userId, int commentId);
}
