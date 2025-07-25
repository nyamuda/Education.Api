namespace Education.Api.Models;

public class UserOtp
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required int UserId { get; set; }

    public User? User { get; set; }

    public required string Otp { get; set; }

    public required DateTime ExpirationTime { get; set; }

    public required bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
