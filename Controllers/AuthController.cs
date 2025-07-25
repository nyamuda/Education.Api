using Education.Api.Dtos.Auth;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    // POST api/<AccountController>/register
    [HttpPost("register")]
    public async Task<IActionResult> Post(RegisterDto registerDto)
    {
        try
        {
            var user = await _authService.RegisterAsync(registerDto);

            return CreatedAtRoute(
                routeName: "GetUserById",
                routeValues: new { id = user.Id },
                value: user
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(details: ex.Message));
        }
    }

    // POST api/<AccountController>/login
    [HttpPost("login")]
    public async Task<IActionResult> Post(LoginDto loginDto)
    {
        try
        {
            var (accessToken, refreshToken) = await _authService.LoginAsync(loginDto);

            //Create an HTTP-Only cookie to store the refresh token
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

            return Ok(new { token = accessToken });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(details: ex.Message));
        }
    }

    [HttpPost("password-reset/request")]
    public async Task<IActionResult> RequestPasswordReset(PasswordResetRequestDto dto)
    {
        try
        {
            await _authService.RequestPasswordResetAsync(dto.Email);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(details: ex.Message));
        }
    }

    [HttpPost("password-reset/reset")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        try
        {
            //reset the password of the user
            await _authService.ResetPasswordAsync(dto);

            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(details: ex.Message));
        }
    }

    //Send email verification email
    [HttpPost("email-verification/request")]
    public async Task<IActionResult> EmailVerificationRequest(EmailVerificationRequestDto dto)
    {
        try
        {
            await _authService.RequestEmailVerificationAsync(dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(details: ex.Message));
        }
    }

    //verify email by validating otp code
    [HttpPut("email-verification/verify")]
    public async Task<IActionResult> VerifyEmail(VerifyOtpDto dto)
    {
        try
        {
            //verify the user email
            await _authService.VerifyEmailAsync(dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(details: ex.Message));
        }
    }

    // POST api/<AccountController>/refresh-token
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            //Get the refresh token from the HTTP-Only cookie
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException(
                    "Access denied: refresh token is missing from the request."
                );

            // Validate the refresh token and retrieve the user details associated with it
            (int tokenUserId, _, _) = _jwtService.ValidateTokenAndExtractUser(refreshToken);

            //generate a new access token for the user
            string token = await _authService.RefreshTokenAsync(userId: tokenUserId);

            return Ok(token);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ErrorResponse.Create(ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ErrorResponse.Create(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyDetails()
    {
        try
        {
            // First, get the access token for the authorized user
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            // Validate the token and retrieve the user details associated with it
            (int tokenUserId, _, _) = _jwtService.ValidateTokenAndExtractUser(token);

            //fetch all the details about the user
            UserDto user = await _userService.GetByIdAsync(tokenUserId);

            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new ErrorResponse
                {
                    Message = ErrorMessageHelper.UnexpectedErrorMessage,
                    Details = ex.Message
                }
            );
        }
    }
}
