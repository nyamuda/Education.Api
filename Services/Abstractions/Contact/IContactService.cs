using Education.Api.Dtos.Contact;

namespace Education.Api.Services.Abstractions.Contact;

/// <summary>
/// Service interface for sending a message sent via the contact us form on the frontend
/// to the admin.
/// </summary>
public interface IContactService
{
    Task SendAsync(ContactDto dto);
}
