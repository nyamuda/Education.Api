using Education.Api.Enums;
using Education.Api.Models.Users;

namespace Education.Api.Dtos.Users;

public class UserDto
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required UserRole Role { get; set; }

    public required bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public static UserDto MapFrom(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsVerified = user.IsVerified,
            CreatedAt = user.CreatedAt,
        };
    }
}
