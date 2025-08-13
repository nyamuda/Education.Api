using Education.Api.Dtos.Auth;
using Education.Api.Dtos.Users;
using Education.Api.Models.Users;

namespace Education.Api.Services.Abstractions.Auth;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto userRegisterDto);

    Task<(string accessToken, string refreshToken)> LoginAsync(LoginDto loginDto);

    Task RequestPasswordResetAsync(string email);

    Task<string> VerifyOtpAndGenerateResetToken(VerifyOtpDto dto);

    Task ResetPasswordAsync(ResetPasswordDto dto);

    Task RequestEmailVerificationAsync(EmailVerificationRequestDto dto);

    Task VerifyEmailAsync(VerifyOtpDto verifyOtpDto);

    Task<string> RefreshTokenAsync(int userId);

    Task<string> GenerateUniqueUsernameAsync(string username, int maxAttempts = 10);
}
