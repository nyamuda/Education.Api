namespace Education.Api.Models;

/// <summary>
/// Represents the configuration settings required to send emails via SMTP.
/// </summary>
public class SmtpSettings
{
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string Password { get; set; }
    public required string Host { get; set; }
}
