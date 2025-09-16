using Education.Api.Dtos.Upvotes;

namespace Education.Api.Services.Abstractions.Upvotes;

/// <summary>
/// Service interface for managing upvotes for questions, answers, and comments.
/// </summary>
public interface IUpvoteService
{
    /// <summary>
    /// Adds an upvote to the specified question by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user upvoting the question.</param>
    /// <param name="questionId">The ID of the question to upvote.</param>
    Task UpvoteQuestionAsync(int userId, int questionId);

    /// <summary>
    /// Removes an existing upvote from the specified question by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user removing their upvote.</param>
    /// <param name="questionId">The ID of the question to remove the upvote from.</param>
    Task RemoveQuestionUpvoteAsync(int userId, int questionId);

    /// <summary>
    /// Retrieves upvotes for a specified question.
    /// </summary>
    /// <param name="questionId">The ID of the question to fetch the upvotes for.</param>
    /// <returns>All the upvotes for the question with the given ID.</returns>
    Task<List<UpvoteDto>> GetQuestionUpvotesAsync(int questionId);

    /// <summary>
    /// Adds an upvote to the specified answer by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user upvoting the answer.</param>
    /// <param name="answerId">The ID of the answer to upvote.</param>
    Task UpvoteAnswerAsync(int userId, int answerId);

    /// <summary>
    /// Removes an existing upvote from the specified answer by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user removing their upvote.</param>
    /// <param name="answerId">The ID of the answer to remove the upvote from.</param>
    Task RemoveAnswerUpvoteAsync(int userId, int answerId);

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
