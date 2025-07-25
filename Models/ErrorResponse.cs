namespace Education.Api.Models;

// A simple model used to represent error responses in the API
public class ErrorResponse
{
    // A short message describing the error
    public required string Message { get; set; }

    // Optional detailed explanation
    public string? Details { get; set; }

    // Factory method for generating a general unexpected error response
    public static ErrorResponse Unexpected(string? details) =>
        new()
        {
            Message = "The server encountered an unexpected issue. Please try again later.",
            Details = details
        };

    // Factory method for generating a 403 Forbidden error response
    public static ErrorResponse Forbidden(string? details) =>
        new()
        {
            Message = "You do not have permission to access this resource.",
            Details = details
        };

    // Factory method for missing NameIdentifier claim in the token
    public static ErrorResponse MissingNameIdentifierClaim(string? details) =>
        new()
        {
            Message = "Access denied. Token lacks a valid name identifier claim.",
            Details = details
        };

    // Factory method for missing Email claim in the token
    public static ErrorResponse MissingEmailClaimMessage(string? details) =>
        new() { Message = "Access denied. Token lacks a valid email claim.", Details = details };

    // Factory method for missing Role claim in the token
    public static ErrorResponse MissingRoleClaimMessage(string? details) =>
        new() { Message = "Access denied. Token lacks a valid role claim.", Details = details };
}
