namespace Education.Api.Helpers;

public class ErrorMessageHelper
{
    public static string UnexpectedErrorMessage { get; } =
        "The server encountered an unexpected issue. Please try again later.";

    public static string ForbiddenErrorMessage { get; } =
        "You do not have permission to access this resource.";

    public static string MissingNameIdentifierClaimMessage { get; } =
        "Access denied. Token lacks a valid name identifier claim.";

    public static string MissingEmailClaimMessage { get; } =
        "Access denied. Token lacks a valid email claim.";

    public static string MissingRoleClaimMessage { get; } =
        "Access denied. Token lacks a valid role claim.";
}
