using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Auth;
using Microsoft.IdentityModel.Tokens;

namespace DrivingSchool.API.Services.Implementations.Auth;

public class JwtService : IJwtService
{
    private readonly string _jwtIssuer = string.Empty;
    private readonly string _jwtAudience = string.Empty;
    private readonly SymmetricSecurityKey _securityKey;

    public JwtService(IConfiguration config)
    {
        //Get the JWT settings
        _jwtIssuer =
            config.GetValue<string>("Authentication:JwtSettings:Issuer")
            ?? throw new KeyNotFoundException("JWT Issuer not found in configuration.");
        _jwtAudience =
            config.GetValue<string>("Authentication:JwtSettings:Audience")
            ?? throw new KeyNotFoundException("JWT Audience not found in configuration.");

        var key =
            config.GetValue<string>("Authentication:JwtSettings:Key")
            ?? throw new KeyNotFoundException("JWT Key not found in configuration.");

        //Encode the key
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    //Create a new JWT Token
    public string GenerateJwtToken(User user, double expiresInMinutes = 10)
    {
        //Sign the key
        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("isVerified", user.IsVerified.ToString())
            ],
            //default token lifespan is  = 10 minutes
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates a JWT token and extracts the authenticated user's ID, email, and role.
    /// </summary>
    /// <param name="token">The JWT token to validate and decode.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    ///   <item><c>int UserId</c>: extracted from <c>ClaimTypes.NameIdentifier</c></item>
    ///   <item><c>string UserEmail</c>: extracted from <c>ClaimTypes.Email</c></item>
    ///   <item><c>UserRole UserRole</c>: determined from <c>ClaimTypes.Role</c></item>
    /// </list>
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if any of the required claims (NameIdentifier, Email, Role) are missing or malformed.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the token is invalid, expired, or fails validation.
    /// </exception>
    public (int UserId, string UserEmail, UserRole UserRole) ValidateTokenAndExtractUser(
        string token
    )
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // The rules that the JWT must comply with to be considered valid.
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Ensures the token hasn't expired
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtIssuer,
            ValidAudience = _jwtAudience,
            IssuerSigningKey = _securityKey
        };

        // Validate and decode the JWT
        ClaimsPrincipal claimsPrincipal =
            tokenHandler.ValidateToken(
                token,
                validationParameters,
                out SecurityToken validatedToken
            )
            ?? throw new InvalidOperationException("The provided token is invalid or has expired.");

        // Extract the user ID from the claims
        string userId =
            claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException(
                ErrorMessageHelper.MissingNameIdentifierClaimMessage
            );

        // Extract the email from the claims
        string userEmail =
            claimsPrincipal.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedAccessException(ErrorMessageHelper.MissingEmailClaimMessage);

        // Extract the role from the claims
        string userRole =
            claimsPrincipal.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException(ErrorMessageHelper.MissingRoleClaimMessage);

        // Convert the user ID to an integer
        if (int.TryParse(userId, out int id))
        {
            var role = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                ? UserRole.Admin
                : UserRole.User;

            return (id, userEmail, role);
        }

        // If userId claim exists but isn't a valid integer, treat it as invalid
        throw new UnauthorizedAccessException(ErrorMessageHelper.MissingNameIdentifierClaimMessage);
    }
}
