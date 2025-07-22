using DrivingSchool.API.Dtos;
using DrivingSchool.API.Dtos.Account;

namespace DrivingSchool.API.Services.Abstractions.Auth;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto userRegisterDto);

    Task<(string accessToken, string refreshToken)> LoginAsync(LoginDto loginDto);

    Task RequestPasswordResetAsync(string email);

    Task ResetPasswordAsync(string email, string newPassword);

    Task RequestEmailVerificationAsync(string emailToBeVerified);

    Task VerifyEmailAsync(int userId, string verifiedEmail);

    Task<string> RefreshTokenAsync(int userId);
}
