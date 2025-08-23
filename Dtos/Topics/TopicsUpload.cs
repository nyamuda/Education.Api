using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Topics;

public class TopicsUpload
{
    [Required]
    public required IFormFile File { get; set; }
}
