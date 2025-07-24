using System.Security.Cryptography;
using Education.Api.Data;
using Education.Api.Dtos.Auth;
using Education.Api.Services.Abstractions.Auth;

namespace Education.Api.Services.Implementations.Auth;

public class OtpService : IOtpService
{
    public ApplicationDbContext _context;

    public OtpService(ApplicationDbContext context)
    {
        _context = context;
    }

    //Generate a secure six figure random one time password (OTP)
    public string GenerateRandomOtp()
    {
        // Generate a cryptographically secure random integer number between 0 and 999 999
        int randomNumber = RandomNumberGenerator.GetInt32(0, 1_000_000);

        //Make sure the OTP always has six digits
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
    public async Task VerifyUserOtp(VerifyOtpDto dto)
    {
        // Retrieve the most recent valid OTP that hasn't expired or been used
        var userOtp = await _context
            .UserOtps
            .Where(
                o =>
                    o.Email.Equals(otpVerificationDto.Email)
                    && o.ExpirationTime > DateTime.UtcNow
                    && !o.IsUsed
            )
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (userOtp is null)
            throw new InvalidOperationException(
                "Your code has expired or is invalid. Please request a new one."
            );

        //The saved OTP code is hashed
        //Check if the provided OTP matched the saved one
        bool isOptCorrect = BCrypt.Net.BCrypt.Verify(otpVerificationDto.OtpCode, userOtp.OtpCode);
        if (!isOptCorrect)
            throw new UnauthorizedAccessException(
                "We couldn't verify your OTP. Double-check the code and try again."
            );

        //mark the OTP as used
        userOtp.IsUsed = true;

        await _context.SaveChangesAsync();
    }
}
