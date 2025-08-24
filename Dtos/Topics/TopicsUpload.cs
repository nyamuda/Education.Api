using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Topics;

/// <summary>
/// Represents the model used for uploading a JSON file that contains topics and subtopics.
/// </summary>
public class TopicsUpload
{
    [Required]
    public required IFormFile File { get; set; }
}
