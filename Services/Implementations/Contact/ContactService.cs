using Education.Api.Data;
using Education.Api.Dtos.Contact;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Contact;
using Education.Api.Services.Abstractions.Email;
using Microsoft.Extensions.Options;

namespace Education.Api.Services.Implementations.Contact;

public class ContactService(
    ApplicationDbContext context,
    ILogger<ContactService> logger,
    IEmailService emailService,
    IEmailTemplateBuilder templateBuilder,
    IOptions<Company> options
) : IContactService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ContactService> _logger = logger;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailTemplateBuilder _emailTemplateBuilder = templateBuilder;
    private readonly Company _company = options.Value;

    /// <summary>
    /// Sends an email to an admin containing a message sent by a client on the frontend
    /// via the contact us form.
    /// </summary>
    /// <param name="dto">The message details sent by the client.</param>
    public async Task SendAsync(ContactDto dto)
    {
        //built the email body
        string htmlBody = _emailTemplateBuilder.BuildContactFormMessageTemplate(dto);

        EmailMessage emailMessage =
            new()
            {
                RecipientName = _company.Name,
                RecipientEmail = _company.Email,
                Subject = "Contact Form Message",
                HtmlBody = htmlBody
            };

        //send the email
        await _emailService.SendAsync(emailMessage);
    }
}
