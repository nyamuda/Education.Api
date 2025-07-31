using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Topics;

public class UpdateTopicDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Please select at least one subject.")]
    public required List<int> SubjectIds { get; set; }
}
