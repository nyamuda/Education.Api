using Education.Api.Dtos.Comments;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Comments;

/// <summary>
/// Service interface for managing comments on questions.
/// </summary>
public interface IQuestionCommentService
{
    Task<PageInfo<CommentDto>> GetAsync(int questionId, int page, int pageSize);

    Task<CommentDto> AddAsync(int userId, int questionId, AddCommentDto dto);
}
