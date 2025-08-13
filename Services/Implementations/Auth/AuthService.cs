using System.Security.Cryptography;
using Education.Api.Data;
using Education.Api.Dtos.Auth;
using Education.Api.Dtos.Users;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Models.Users;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Email;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Auth;

public class AuthService(
    ApplicationDbContext context,
    IJwtService jwtService,
    IOtpService otpService,
    IEmailService emailService,
    IEmailTemplateBuilder emailTemplateBuilder,
    ILogger<AuthService> logger
) : IAuthService
{
    protected readonly ApplicationDbContext _context = context;
    protected readonly IJwtService _jwtService = jwtService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailTemplateBuilder _emailTemplateBuilder = emailTemplateBuilder;
    private readonly IOtpService _otpService = otpService;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<UserDto> RegisterAsync(RegisterDto dto)
    {
        // check if a user with the provided email already exists
        bool userExists = await _context.Users.AnyAsync(u => u.Email.Equals(dto.Email));
        if (userExists)
        {
            _logger.LogWarning(
                "Registration failed: user with email {Email} already exists.",
                dto.Email
            );
            throw new ConflictException(
                "An account with this email already exists. Try signing in or use a different email to register."
            );
        }
        //Make sure the provided username is unique
        var uniqueUsername = await GenerateUniqueUsernameAsync(username: dto.Username);

        // hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        //build the user entity
        User user = await BuildUserAsync(
            dto: dto,
            uniqueUsername: uniqueUsername,
            hashedPassword: hashedPassword
        );

        //Add new user to the database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Registration successful: added new user with email {Email}.",
            dto.Email
        );

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
            ?? throw new KeyNotFoundException("Invalid email or password.");

        // Compare the provided password with the stored hashed password
        string hashedPassword = user.Password;
        var isCorrectPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, hashedPassword);

        if (!isCorrectPassword)
        {
            _logger.LogWarning(
                "Login failed: invalid credentials for user with email {Email}.",
                loginDto.Email
            );

            throw new UnauthorizedAccessException("Invalid email or password.");
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

        _logger.LogInformation(
            "Login successful: user with with email {Email} is now logged in.",
            loginDto.Email
        );

        return (accessToken, refreshToken);
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        //check to see if user with the given email exists
        var existingUser = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Equals(email));

        // if the specified user does not exist,
        // quietly abort the password reset request operation
        if (existingUser is null)
        {
            _logger.LogWarning(
                "Password reset request failed: user with email {Email} does not exist.",
                email
            );

            return;
        }

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

        _logger.LogInformation("Successfully sent a password reset OTP to email {Email}", email);
    }

    /// <summary>
    /// Verifies the password reset OTP and generates a secure JWT token for password reset if the OTP is valid.
    /// </summary>
    /// <param name="dto">DTO containing the user's email and OTP.</param>
    /// <returns>A JWT token that can be used to reset the user's password.</returns>
    public async Task<string> VerifyOtpAndGenerateResetToken(VerifyOtpDto dto)
    {
        //verify OTP
        await _otpService.VerifyAsync(dto);

        //If the OTP is valid,
        //generate the password reset token and send it to the client
        var user =
            await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(dto.Email))
            ?? throw new KeyNotFoundException(
                $@"Password reset OTP verification failed. User with email ""{dto.Email}"" does not exist."
            );

        var resetToken = _jwtService.GenerateJwtToken(user: user, expiresInMinutes: 15);

        return resetToken;
    }

    //Resets password by validating the reset token
    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        //Validate the reset token and extract the user details associated with it
        (int userId, _, _) = _jwtService.ValidateTokenAndExtractUser(dto.ResetToken);

        //check if user with the given email exists
        var existingUser =
            await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId))
            ?? throw new InvalidOperationException(
                "Unable to reset password: no user found for the provided reset token."
            );

        //hash the new password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        //update the password
        existingUser.Password = hashedPassword;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Password successfully reset for user: {UserId}", userId);
    }

    public async Task RequestEmailVerificationAsync(EmailVerificationRequestDto dto)
    {
        //check to see if user with the given email exists
        var existingUser = await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Equals(dto.Email));

        // if the specified user does not exist,
        // quietly abort the email verification request operation
        if (existingUser is null)
        {
            _logger.LogWarning(
                "Email verification request failed: user with email {Email} does not exist.",
                dto.Email
            );

            return;
        }
        //check if the user email hasn't been already verified
        if (existingUser.IsVerified)
        {
            _logger.LogWarning(
                "Verification not required — email {email} already confirmed.",
                dto.Email
            );
            throw new ConflictException("This email has already been verified.");
        }
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

        _context.UserOtps.Add(userOtp);
        await _context.SaveChangesAsync();

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

        _logger.LogInformation(
            "Successfully sent an email verification OTP to email {Email}",
            dto.Email
        );
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
            await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(verifyOtpDto.Email))
            ?? throw new KeyNotFoundException(
                $"Email verification failed: user with email '{verifyOtpDto.Email}' does not exist."
            );

        // Mark the user as verified
        user.IsVerified = true;

        // Save the changes to the database
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully verified email {Email}", verifyOtpDto.Email);
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

    /// <summary>
    /// Ensures that the provided username is unique in the database.
    /// If it already exists, appends a cryptographically random number
    /// and retries until a unique username is found or the maximum number of attempts is reached.
    /// </summary>
    /// <param name="username">The base username to check.</param>
    /// <param name="maxAttempts">The maximum number of attempts to generate a unique username.</param>
    /// <returns>A unique username guaranteed not to exist in the database at the time of check.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a unique username cannot be generated after the maximum number of attempts.
    /// </exception>
    public async Task<string> GenerateUniqueUsernameAsync(string username, int maxAttempts = 10)
    {
        string candidate = username;
        int attempt = 0;

        while (await _context.Users.AnyAsync(u => u.Username == candidate) && attempt < maxAttempts)
        {
            int randomValue = RandomNumberGenerator.GetInt32(0, 1_000_000); // 0 to 999999
            candidate = $"{username}{randomValue}";
            attempt++;
        }

        if (await _context.Users.AnyAsync(u => u.Username == candidate))
        {
            _logger.LogWarning(
                "Unable to generate a unique username after {MaxAttempts} attempts. Last attempted: {Username}",
                maxAttempts,
                candidate
            );
            throw new InvalidOperationException("Unable to generate a unique username.");
        }

        return candidate;
    }

    /// <summary>
    /// Builds a new User entity with the provided registration details.
    /// Ensures Curriculum, Exam Board, and Levels are valid and related.
    /// </summary>
    /// <param name="dto">Registration request DTO.</param>
    /// <param name="uniqueUsername">The already-generated unique username.</param>
    /// <param name="hashedPassword">The hashed password string.</param>
    /// <returns>A fully constructed User entity.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when curriculum, exam board, or levels are not found.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when levels do not match the selected exam board.
    /// </exception>
    private async Task<User> BuildUserAsync(
        RegisterDto dto,
        string uniqueUsername,
        string hashedPassword
    )
    {
        var user = new User
        {
            Username = uniqueUsername,
            Email = dto.Email,
            Password = hashedPassword,
            IsVerified = false,
        };

        // Validate curriculum
        if (dto.CurriculumId is not null && dto.CurriculumId != 0)
        {
            var curriculum =
                await _context
                    .Curriculums
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == dto.CurriculumId)
                ?? throw new KeyNotFoundException(
                    $"Curriculum with ID '{dto.CurriculumId}' was not found."
                );
            user.CurriculumId = curriculum.Id;

            // Validate exam board
            if (dto.ExamBoardId is not null && dto.ExamBoardId != 0)
            {
                var examBoard =
                    await _context
                        .ExamBoards
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x => x.Id == dto.ExamBoardId && x.CurriculumId == dto.CurriculumId
                        )
                    ?? throw new KeyNotFoundException(
                        $"Exam board with ID '{dto.ExamBoardId}' does not belong to curriculum '{dto.CurriculumId}'."
                    );
                user.ExamBoardId = examBoard.Id;
            }
            // Validate levels
            if (dto.LevelIds.Count > 0)
            {
                var levels = await _context
                    .Levels
                    .Where(x => dto.LevelIds.Contains(x.Id) && x.ExamBoardId == dto.ExamBoardId)
                    .ToListAsync();

                if (levels.Count != dto.LevelIds.Count)
                {
                    throw new InvalidOperationException(
                        "One or more selected educational levels do not exist under the chosen exam board."
                    );
                }

                user.Levels.AddRange(levels);
            }
        }

        return user;
    }
}
