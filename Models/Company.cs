namespace Education.Api.Models;

/// <summary>
/// Represents the company details loaded from the application's configuration settings.
/// </summary>
/// <remarks>
/// This is used to bind the "Company" section using IOptions.
/// </remarks>
public class Company
{
    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string Phone { get; set; }

    public required string WebsiteUrl { get; set; }
}
