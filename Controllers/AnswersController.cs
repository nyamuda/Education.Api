using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Comments;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Answers;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswersController(
    IAnswerService answerService,
    IJwtService jwtService,
    IAnswerCommentService answerCommentService
) : ControllerBase
{
    private readonly IAnswerService _answerService = answerService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IAnswerCommentService _answerCommentService = answerCommentService;

    //Gets an answer by ID
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

    //Updates an answer with a given ID
    [HttpPut("{id}")]
    [Authorize]
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

    //Deletes an answer with a given ID
    [HttpDelete("{id}")]
    [Authorize]
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

    //Gets a paginated list of comments for a given answer
    [HttpGet("{answerId}/comments")]
    public async Task<IActionResult> GetComments(int answerId, int page = 1, int pageSize = 10)
    {
        try
        {
            PageInfo<CommentDto> comments = await _answerCommentService.GetAsync(
                answerId: answerId,
                page: page,
                pageSize: pageSize
            );

            return Ok(comments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Adds a new comment for an answer with a given ID
    [HttpPost("{answerId}/comments")]
    [Authorize]
    public async Task<IActionResult> PostComment(int answerId, AddCommentDto dto)
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

            CommentDto comment = await _answerCommentService.AddAsync(
                userId: userId,
                answerId: answerId,
                dto
            );

            return CreatedAtRoute(
                routeName: "GetCommentById",
                routeValues: new { id = comment.Id },
                value: comment
            );
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
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }
}
