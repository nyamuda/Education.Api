using Education.Api.Models;
using Education.Api.Services.Abstractions.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Education.Api.Services.Implementations.Email;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    // inject IOptions<TOptions> to access the SMTP config settings
    // from the `appsettings.json` file in a strongly typed manner
    public EmailService(IOptions<SmtpSettings> options)
    {
        _smtpSettings = options.Value;
    }

    public async Task SendAsync(EmailMessage emailMessage)
    {
        var messageToSend = new MimeMessage();
        messageToSend
            .From
            .Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        messageToSend
            .To
            .Add(new MailboxAddress(emailMessage.RecipientName, emailMessage.RecipientEmail));
        messageToSend.Subject = emailMessage.Subject;

        //send the body as HTML
        messageToSend.Body = new TextPart("html") { Text = emailMessage.HtmlBody };

        //Connect to the SMTP server
        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpSettings.Host, 587, false);

        //authenticate with the SMTP server using the sender email and password
        await client.AuthenticateAsync(_smtpSettings.SenderEmail, _smtpSettings.Password);

        //send the email
        await client.SendAsync(messageToSend);
        await client.DisconnectAsync(true);
    }
}
