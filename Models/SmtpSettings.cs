namespace Education.Api.Models;

/// <summary>
/// Represents the details required to send emails via SMTP loaded
/// from the application's configuration settings.
/// </summary>
/// <remarks>
/// This is used to bind the "SmtpSettings" section using IOptions.
/// </remarks>
public class SmtpSettings
{
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string Password { get; set; }
    public required string Host { get; set; }
}
