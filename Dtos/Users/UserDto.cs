using Education.Api.Enums;
using Education.Api.Models.Users;

namespace Education.Api.Dtos.Users;

public class UserDto
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public string? Email { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

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
