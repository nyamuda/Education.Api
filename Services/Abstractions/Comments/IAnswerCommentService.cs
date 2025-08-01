using Education.Api.Dtos.Comments;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Comments;

/// <summary>
/// Service interface for managing comments on answers.
/// </summary>
public interface IAnswerCommentService
{
    Task<PageInfo<CommentDto>> GetAsync(int answerId, int page, int pageSize);

    Task<CommentDto> AddAsync(int userId, int answerId, AddCommentDto dto);
}
