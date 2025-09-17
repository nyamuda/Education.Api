using Education.Api.Data;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Bookmarks;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Bookmarks;

public class QuestionBookmarkService(
    ApplicationDbContext context,
    ILogger<QuestionBookmarkService> logger
) : IQuestionBookmarkService
{
    public ApplicationDbContext _context = context;
    public ILogger<QuestionBookmarkService> _logger = logger;

    //Bookmarks a question for a user with a given ID.
    public async Task AddAsync(int userId, int questionId)
    {
        //check if the user exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        if (user == null)
        {
            _logger.LogWarning(
                "Unable to bookmark question {questionId}: user {userId} not found.",
                questionId,
                userId
            );
            throw new KeyNotFoundException(
                $"Failed to bookmark question. User with ID {userId} not found."
            );
        }
        //check if the question exists
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id.Equals(questionId));
        if (question == null)
        {
            _logger.LogWarning(
                "Unable to bookmark question {questionId}: question {questionId} not found.",
                questionId,
                questionId
            );
            throw new KeyNotFoundException(
                $"Failed to bookmark question. Question with ID {questionId} not found."
            );
        }

        //create a bookmark for the question
        QuestionBookmark bookmark = new() { QuestionId = questionId, UserId = userId };

        _context.QuestionBookmarks.Add(bookmark);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "User {userId} successfully bookmarked question {questionId}.",
            userId,
            questionId
        );
    }

    Task DeleteAsync(int userId, int questionId);
}
