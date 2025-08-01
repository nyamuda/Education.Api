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
    Task RemoveQuestionUpvoteAsync(int userId, int questionId);

    Task UpvoteAnswerAsync(int userId, int answerId);
    Task RemoveAnswerUpvoteAsync(int userId, int answerId);

    Task UpvoteCommentAsync(int userId, int commentId);
    Task RemoveCommentUpvoteAsync(int userId, int commentId);
}
