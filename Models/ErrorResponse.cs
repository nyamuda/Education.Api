namespace Education.Api.Models;

public class ErrorResponse
{
    public required string Message { get; set; }

    public string? Details { get; set; }

    public static ErrorResponse Unexpected(string? details) => new() 
    {
        Message="The server encountered an unexpected issue. Please try again later.";
        Details=details;
    };

    public static ErrorResponse Forbidden(string? details) => new()
    {
        Message = "You do not have permission to access this resource.",
        Details = details
    };
    
     public static ErrorResponse MissingNameIdentifierClaim(string? details) => new()
    {
        Message = "Access denied. Token lacks a valid name identifier claim.",
        Details = details
    };
    public static ErrorResponse MissingEmailClaimMessage(string? details) => new()
    {
        Message = "Access denied. Token lacks a valid email claim.",
        Details = details
    };
    
    public static ErrorResponse MissingRoleClaimMessage(string? details) => new()
    {
        Message = "Access denied. Token lacks a valid role claim.",
        Details = details
    };

}
