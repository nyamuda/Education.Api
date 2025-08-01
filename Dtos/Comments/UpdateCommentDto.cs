using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Comments;

public class UpdateCommentDto
{
    [Required]
    public required string Content { get; set; }
}
