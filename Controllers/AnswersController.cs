using Education.Api.Dtos.Answers;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Answers;
using Education.Api.Services.Abstractions.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswersController(IAnswerService answerService, IJwtService jwtService)
    : ControllerBase
{
    private readonly IAnswerService _answerService = answerService;
    private readonly IJwtService _jwtService = jwtService;

    [HttpGet("{id}", Name = "GetAnswerById")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            AnswerDto answer = await _answerService.GetByIdAsync(id);
            return Ok(answer);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UpdateAnswerDto dto)
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

            await _answerService.UpdateAsync(userId: userId, answerId: id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
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

            await _answerService.DeleteAsync(userId: userId, answerId: id);

            return NoContent();
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
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }
}
