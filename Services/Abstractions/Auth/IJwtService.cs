using Education.Api.Enums;
using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Auth;

public interface IJwtService
{
    string GenerateJwtToken(User user, double expiresInMinutes = 10);

    (int UserId, string UserEmail, UserRole UserRole) ValidateTokenAndExtractUser(string token);
}
