namespace Education.Api.Services.Abstractions.Bookmarks;

public interface IQuestionBookmarkService
{
    Task AddAsync(int userId, int questionId);

    Task DeleteAsync(int userId, int questionId);
}
