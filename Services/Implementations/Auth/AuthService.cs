using Education.Api.Data;
using Education.Api.Dtos.Auth;
using Education.Api.Dtos.Users;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Email;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Auth;

public class AuthService : IAuthService
{
    protected readonly ApplicationDbContext _context;
    protected readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    private readonly IEmailTemplateBuilder _emailTemplateBuilder;
    private readonly IOtpService _otpService;

    public AuthService(
        ApplicationDbContext context,
        IJwtService jwtService,
        IOtpService otpService,
        IEmailService emailService,
        IEmailTemplateBuilder emailTemplateBuilder
    )
    {
        _context = context;
        _jwtService = jwtService;
        _emailService = emailService;
        _emailTemplateBuilder = emailTemplateBuilder;
        _otpService = otpService;
    }

    public async Task<UserDto> RegisterAsync(RegisterDto userRegisterDto)
    {
        // check if user with the provided email already exists
        bool userExists = await _context.Users.AnyAsync(u => u.Email.Equals(userRegisterDto.Email));
        if (userExists)
        {
            var message = "A user with this email is already registered.";
            throw new InvalidOperationException(message);
        }

        // hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

        var user = new User
        {
            Username = userRegisterDto.UserName,
            Email = userRegisterDto.Email,
            Password = hashedPassword,
            IsVerified = false,
        };

        //add the user
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return UserDto.MapFrom(user);
    }

    public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginDto loginDto)
    {
        //access token lifespan is 72 hours  = 4320 minutes
        var accessTokenLifespan = 4320;

        //refresh token lifespan is 7 days  = 10080 minutes
        var refreshTokenLifespan = 10080;

        //check if user exists
        var user =
            await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email)
            ?? throw new KeyNotFoundException("Invalid username or password");

        // Compare the provided password with the stored hashed password
        string hashedPassword = user.Password;
        var isCorrectPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, hashedPassword);

        if (!isCorrectPassword)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        //create token since the provided password is correct
        //token lifespan is 72 hours
        var accessToken = _jwtService.GenerateJwtToken(
            user: user,
            expiresInMinutes: accessTokenLifespan
        );

        //create a refresh token
        //token lifespan is 7 days
        var refreshToken = _jwtService.GenerateJwtToken(
            user: user,
            expiresInMinutes: refreshTokenLifespan
        );

        return (accessToken, refreshToken);
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        //check to see if user with the given email exists
        var existingUser =
            await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(email))
            ?? throw new KeyNotFoundException($@"User with email ""{email}"" does not exist.");

        //create the password reset OTP
        string resetOtp = _otpService.Generate();
        //hash the OTP
        string hashedOtp = BCrypt.Net.BCrypt.HashPassword(resetOtp);

        //store the hashed OTP to the database
        UserOtp userOtp =
            new()
            {
                Email = existingUser.Email,
                UserId = existingUser.Id,
                Otp = hashedOtp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10), // expires in 10 minutes
            };

        await _context.UserOtps.AddAsync(userOtp);

        //generate the email template
        string emailTemplate = _emailTemplateBuilder.BuildPasswordResetRequestTemplate(
            recipientName: existingUser.Username,
            otp: resetOtp
        );

        //send email to reset password
        EmailMessage emailMessage =
            new()
            {
                RecipientName = existingUser.Username,
                RecipientEmail = existingUser.Email,
                Subject = "Password Reset Request",
                HtmlBody = emailTemplate,
            };
        await _emailService.SendAsync(emailMessage);
    }

    //Resets password by validating the reset token
    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        //Validate the reset token and extract the user details associated with it
        (_, string userEmail, _) = _jwtService.ValidateTokenAndExtractUser(dto.ResetToken);

        //check if user with the given email exists
        var userExists =
            await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(dto.ResetToken))
            ?? throw new InvalidOperationException(
                "Unable to reset password: no user found for the provided reset token."
            );

        //hash the new password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        //update the password
        userExists.Password = hashedPassword;
        await _context.SaveChangesAsync();
    }

    public async Task RequestEmailVerificationAsync(EmailVerificationRequestDto dto)
    {
        //check to see if user with the given email exists
        var existingUser =
            await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(dto.Email))
            ?? throw new KeyNotFoundException($@"User with email ""{dto.Email}"" does not exist.");

        //create the verification OTP
        string verificationOtp = _otpService.Generate();
        //hash the OTP
        string hashedOtp = BCrypt.Net.BCrypt.HashPassword(verificationOtp);

        //store the hashed OTP to the database
        UserOtp userOtp =
            new()
            {
                Email = existingUser.Email,
                UserId = existingUser.Id,
                Otp = hashedOtp,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10), // expires in 10 minutes
            };

        await _context.UserOtps.AddAsync(userOtp);
        //generate the email template
        string emailTemplate = _emailTemplateBuilder.BuildEmailVerificationRequestTemplate(
            recipientName: existingUser.Username,
            otp: verificationOtp
        );

        //send email to verify email
        EmailMessage emailMessage =
            new()
            {
                RecipientName = existingUser.Username,
                RecipientEmail = existingUser.Email,
                Subject = "Email Confirmation",
                HtmlBody = emailTemplate,
            };
        await _emailService.SendAsync(emailMessage);
    }

    /// <summary>
    /// Verifies a user's email by validating the provided OTP (One-Time Password).
    /// If the OTP is valid, the user's email is marked as verified in the database.
    /// </summary>
    /// <param name="verifyOtpDto">The DTO containing the email and OTP to verify.</param>
    /// <exception cref="KeyNotFoundException">Thrown if no user with the given email is found.</exception>
    public async Task VerifyEmailAsync(VerifyOtpDto verifyOtpDto)
    {
        // Validate the OTP using the OTP service
        await _otpService.VerifyAsync(verifyOtpDto);

        // If validation passes, proceed to find the corresponding user

        // Retrieve the user from the database using the provided email
        var user =
            await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(verifyOtpDto.Email))
            ?? throw new KeyNotFoundException(
                @$"User with email ""{verifyOtpDto.Email}"" does not exist."
            );

        // Mark the user as verified
        user.IsVerified = true;

        // Save the changes to the database
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Generates a new JWT access token for a user with the specified ID.
    /// </summary>
    /// <param name="userId">The ID of the user requesting a new token.</param>
    /// <returns>A newly generated JWT access token.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user with the specified ID does not exist.</exception>
    public async Task<string> RefreshTokenAsync(int userId)
    {
        // Attempt to retrieve the user from the database without tracking changes
        User user =
            await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new KeyNotFoundException($@"User with ID ""{userId}"" does not exist.");

        // Define the token lifespan in minutes (72 hours = 4320 minutes)
        double tokenLifespan = 4320;

        // Generate a new JWT token for the user with the specified expiration time
        string token = _jwtService.GenerateJwtToken(user: user, expiresInMinutes: tokenLifespan);

        return token;
    }
}
