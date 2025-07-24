using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Email;

public interface IEmailService
{
    Task SendAsync(EmailMessage emailMessage);
}
