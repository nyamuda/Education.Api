namespace Education.Api.Models;

public class EmailMessage
{
    public required string RecipientName { get; set; }
    public required string RecipientEmail { get; set; }
    public required string Subject { get; set; }
    public required string HtmlBody { get; set; }
}
