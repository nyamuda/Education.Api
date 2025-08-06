using System.ComponentModel.DataAnnotations;

namespace Education.Api.Dtos.Levels;

public class AddLevelDto
{
    [Required]
    public required string Name { get; set; }
}
