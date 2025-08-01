using Education.Api.Dtos.Comments;

namespace Education.Api.Services.Abstractions.Comments;

public interface ICommentService
{
    Task<CommentDto> GetByIdAsync(int id);

    Task UpdateAsync(int userId, int commentId, UpdateCommentDto dto);

    Task DeleteAsync(int userId, int commentId);
}
