using Education.Api.Data;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Bookmarks;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Bookmarks;

public class QuestionBookmarkService(
    ApplicationDbContext context,
    ILogger<QuestionBookmarkService> logger
) : IQuestionBookmarkService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<QuestionBookmarkService> _logger = logger;

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
                "Unable to bookmark question: question {questionId} not found.",
                questionId
            );
            throw new KeyNotFoundException(
                $"Failed to bookmark question. Question with ID {questionId} not found."
            );
        }
        //check if there isn't already an existing bookmark for the same question by the same user
        bool hasAlreadyBookmarked = await _context
            .QuestionBookmarks
            .Where(x => x.UserId.Equals(userId) && x.QuestionId.Equals(questionId))
            .AnyAsync();

        if (hasAlreadyBookmarked)
        {
            _logger.LogWarning(
                "Bookmark ignored: User {UserId} has already bookmarked question {QuestionId}.",
                userId,
                questionId
            );
            throw new ConflictException(
                $"Duplicate bookmark: User with ID '{userId}' has already bookmarked question with ID '{questionId}'."
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

    //Removes a bookmarked question for a user with a given ID.
    public async Task DeleteAsync(int userId, int questionId)
    {
        //check if the bookmark exists
        var bookmark = await _context
            .QuestionBookmarks
            .FirstOrDefaultAsync(
                qb => qb.UserId.Equals(userId) && qb.QuestionId.Equals(questionId)
            );

        if (bookmark is null)
        {
            _logger.LogWarning(
                "Unable to remove bookmark: bookmark for user {userId} and question {questionId} not found.",
                userId,
                questionId
            );
            throw new KeyNotFoundException(
                $"Failed to remove bookmark. Bookmark for user with ID {userId} and question with ID {questionId} does not exist."
            );
        }
        //remove bookmark
        _context.QuestionBookmarks.Remove(bookmark);
        await _context.SaveChangesAsync();
    }
}
