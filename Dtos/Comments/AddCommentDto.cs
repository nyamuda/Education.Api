using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Comments;

public class AddCommentDto
{
    [Required]
    public required string Content { get; set; }
}
