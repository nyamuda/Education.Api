


public class AuthService : IAuthService
{
    protected readonly ApplicationDbContext _context;
    protected readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly string _frontendUrl;

    public AuthService(
        ApplicationDbContext context,
        IJwtService jwtService,
        IConfiguration config,
        IEmailService emailService
    )
    {
        _context = context;
        _jwtService = jwtService;
        _emailService = emailService;

        //get the frontend url
        _frontendUrl =
            config.GetValue<string>("Frontend:BaseUrl")
            ?? throw new KeyNotFoundException(
                "Frontend URL is missing in application config settings."
            );
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
            Name = userRegisterDto.Name,
            Email = userRegisterDto.Email,
            Phone = userRegisterDto.Phone,
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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
        {
            var message = "User with the provided email does not exist.";
            throw new KeyNotFoundException(message);
        }

        // Compare the provided password with the stored hashed password
        string hashedPassword = user.Password;
        var isCorrectPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, hashedPassword);

        if (!isCorrectPassword)
        {
            var message = "The provided password is incorrect.";
            throw new UnauthorizedAccessException(message);
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

        //generate reset token
        var token = _jwtService.GenerateJwtToken(user: existingUser);

        //create the reset URL
        string resetUrl = $"{_frontendUrl}/auth/password-reset/reset?token={token}";

        // Fetch the company details for branding and contact information
        Company company =
            await _context.Companies.AsNoTracking().FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException("Missing company details. Unable to proceed.");

        string htmlTemplate = HtmlTemplatesHelper.PasswordResetRequestTemplate(
            resetUrl: resetUrl,
            recipientName: existingUser.Name,
            companyName: company.Name,
            companyEmail: company.Email,
            companyAddress: company.Address
        );

        //send email to reset password
        EmailMessage emailMessage =
            new()
            {
                RecipientName = existingUser.Name,
                RecipientEmail = existingUser.Email,
                Subject = "Password Reset Request",
                HtmlBody = htmlTemplate,
            };
        await _emailService.SendAsync(emailMessage);
    }

    //Reset password by validating token
    public async Task ResetPasswordAsync(string email, string newPassword)
    {
        //check if user with the provided email already exists
        var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        if (userExists == null)
        {
            var message = "User with the provided email does not exists.";
            throw new InvalidOperationException(message);
        }

        //hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

        //update the password
        userExists.Password = hashedPassword;
        await _context.SaveChangesAsync();
    }

    public async Task RequestEmailVerificationAsync(string emailToBeVerified)
    {
        //check to see if user with the given email exists
        var existingUser =
            await _context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Equals(emailToBeVerified))
            ?? throw new KeyNotFoundException(
                $"User with email {emailToBeVerified} does not exist."
            );

        // Temporarily assign the new email address to the user's record.
        // This is necessary in cases where the user is updating their existing email address and need to verify the new one.
        // This allows us to include it in the verification token without saving it to the database yet.
        // Once the user clicks the verification link, we will confirm the token, extract that new email and then persist it to the database .
        existingUser.Email = emailToBeVerified;

        //generate reset token
        //expires in 24 hours
        float tokenExpirationInMinutes = 1440F;
        var token = _jwtService.GenerateJwtToken(
            user: existingUser,
            expiresInMinutes: tokenExpirationInMinutes
        );

        //create the verificationUrl URL
        string verificationUrl = $"{_frontendUrl}/auth/email-verification/verify?token={token}";

        // Fetch the company details for branding and contact information
        Company company =
            await _context.Companies.AsNoTracking().FirstOrDefaultAsync()
            ?? new Company
            {
                Name = "Loyd School of Driving",
                Email = "helloworld@gmail.com",
                Phone = "0815896615",
                DateFounded = DateTime.UtcNow,
                Address = "South Africa"
            };

        string htmlTemplate = HtmlTemplatesHelper.EmailVerificationRequestTemplate(
            verificationUrl: verificationUrl,
            recipientName: existingUser.Name,
            companyName: company.Name,
            companyEmail: company.Email,
            companyAddress: company.Address
        );

        //send email to reset password
        EmailMessage emailMessage =
            new()
            {
                RecipientName = existingUser.Name,
                RecipientEmail = existingUser.Email,
                Subject = "Email Confirmation",
                HtmlBody = htmlTemplate,
            };
        await _emailService.SendAsync(emailMessage);
    }

    public async Task VerifyEmailAsync(int userId, string verifiedEmail)
    {
        //check to see if user with the given ID exists
        var userExists =
            await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId))
            ?? throw new KeyNotFoundException("User with the provided ID does not exist.");

        //mark the user as verified
        userExists.IsVerified = true;
        //save the verified email (in case this was an email update and the new email needed to be updated)
        userExists.Email = verifiedEmail;
        await _context.SaveChangesAsync();
    }

    //Generate a new token for a user with a given ID
    public async Task<string> RefreshTokenAsync(int userId)
    {
        //check if user with given ID exists
        User user =
            await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.Equals(userId))
            ?? throw new KeyNotFoundException($@"User with ID ""{userId}"" does not exist.");
        //generate the access token
        //access token lifespan is 72 hours = 4320 minutes
        double tokenLifespan = 4320;
        string token = _jwtService.GenerateJwtToken(user: user, expiresInMinutes: tokenLifespan);
        return token;
    }
}
