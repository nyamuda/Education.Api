namespace Education.Api.Models.Users;

public class UserOtp
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required int UserId { get; set; }

    public User? User { get; set; }

    public required string Otp { get; set; }

    public required DateTime ExpirationTime { get; set; }

    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
