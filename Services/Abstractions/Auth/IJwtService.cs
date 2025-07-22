namespace Education.API.Services.Abstractions.Auth;

public interface IJwtService
{
    string GenerateJwtToken(User user, double expiresInMinutes = 10);

    (int UserId, string UserEmail, UserRole UserRole) ValidateTokenAndExtractUser(string token);
}
