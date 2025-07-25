using Education.Api.Dtos.Contact;

namespace Education.Api.Services.Abstractions.Email;

public interface IEmailTemplateBuilder
{
    string BuildPasswordResetRequestTemplate(string recipientName, string otp);

    string BuildEmailVerificationRequestTemplate(string recipientName, string otp);

    string BuildContactFormMessageTemplate(ContactDto contactDto);
}
