using Education.Api.Data;
using Education.Api.Services.Abstractions.Bookmarks;

namespace Education.Api.Services.Implementations.Bookmarks;

public class QuestionBookmarkService(
    ApplicationDbContext context,
    ILogger<QuestionBookmarkService> logger
) : IQuestionBookmarkService
{
    public ApplicationDbContext _context = context;
    public ILogger<QuestionBookmarkService> _logger = logger;

    public async Task AddAsync(int userId, int questionId) 
    {
        
    }

    Task DeleteAsync(int userId, int questionId);
}
