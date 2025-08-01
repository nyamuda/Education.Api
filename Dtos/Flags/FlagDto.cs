using Education.Api.Dtos.Users;
using Education.Api.Enums;

namespace Education.Api.Dtos.Flags;

public class FlagDto
{
    public required int Id { get; set; }

    public string? Content { get; set; }

    /// <summary>
    /// ID of the user who submitted the flag.
    /// </summary>
    public required int UserId { get; set; }
    public UserDto? User { get; set; }

    /// <summary>
    /// The current status of the flag (e.g., pending review, resolved).
    /// </summary>
    public required FlagStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
