using System.Security.Cryptography;
using Education.Api.Data;
using Education.Api.Dtos.Auth;
using Education.Api.Services.Abstractions.Auth;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Auth;

public class OtpService(ApplicationDbContext context, ILogger<OtpService> logger) : IOtpService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<OtpService> _logger = logger;

    /// <summary>
    /// Generates a cryptographically secure 6-digit one-time password (OTP).
    /// </summary>
    /// <returns>A 6-character numeric OTP as a string, padded with leading zeros if necessary.</returns>
    public string Generate()
    {
        // Generate a cryptographically secure random integer number between 0 and 999 999
        int randomNumber = RandomNumberGenerator.GetInt32(0, 1_000_000);

        // Convert the number to a 6-digit string with leading zeros if needed (e.g., "004582")
        string optValue = randomNumber.ToString("D6");

        return optValue;
    }

    /// <summary>
    /// Verifies a one-time password (OTP) submitted by the user by checking against the most recent active and unused OTP.
    /// </summary>
    /// <param name="VerifyOtpDto">The DTO containing the user's email and the OTP to verify.</param>
    /// <exception cref="KeyNotFoundException">Thrown if no user is found with the provided email.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no valid OTP is found (expired or already used).</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if the provided OTP does not match the saved code.</exception>
    /// <remarks>
    /// This method ensures that only the latest unused and unexpired OTP is checked. Stored OTPs are hashed for security.
    /// </remarks>
    public async Task VerifyAsync(VerifyOtpDto dto)
    {
        // Retrieve the most recent valid OTP that hasn't expired or been used
        var userOtp =
            await _context
                .UserOtps
                .Where(
                    uo =>
                        uo.Email.Equals(dto.Email)
                        && uo.ExpirationTime > DateTime.UtcNow
                        && !uo.IsUsed
                )
                .OrderByDescending(uo => uo.CreatedAt)
                .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException(
                "Your code has expired or is invalid. Please request a new one."
            );

        //The saved OTP code is hashed
        //Check if the provided OTP matched the saved one
        bool isOptCorrect = BCrypt.Net.BCrypt.Verify(dto.Otp, userOtp.Otp);
        if (isOptCorrect)
        {
            //mark the OTP as used
            userOtp.IsUsed = true;

            await _context.SaveChangesAsync();

            _logger.LogInformation("OTP verification successful for {Email}", dto.Email);

            return;
        }

        _logger.LogWarning("OTP verification failed for {Email}", dto.Email);

        throw new UnauthorizedAccessException(
            "We couldn't verify your OTP. Double-check the code and try again."
        );
    }
}
