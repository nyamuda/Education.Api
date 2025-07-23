namespace Education.Api.Services.Abstractions.Auth;

public interface IOtpService
{
    string GenerateCode();
    Task VerifyCode();
}
