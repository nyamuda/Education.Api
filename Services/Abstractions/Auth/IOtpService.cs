using Education.Api.Dtos.Auth;

namespace Education.Api.Services.Abstractions.Auth;

public interface IOtpService
{
    string Generate();
    Task VerifyAsync(VerifyOtpDto dto);
}
