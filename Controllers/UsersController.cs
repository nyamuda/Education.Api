using Education.Api.Models;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService, IJwtService jwtService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IJwtService _jwtService = jwtService;

    // Retrieves the authenticated user's details by ID
    [HttpGet("{id}", Name = "GetUserById")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            //retrieve the access token
            string token = HttpContext
                .Request
                .Headers
                .Authorization
                .ToString()
                .Replace("Bearer ", "");

            //Validate the token and get the details of the user associated with it
            (int userId, _, _) = _jwtService.ValidateTokenAndExtractUser(token);

            //Make sure users can only access their own account information
            if (userId != id)
                return Forbid(ErrorResponse.Forbidden().Message);

            var user = await _userService.GetByIdAsync(id);

            return Ok(user);
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
}
