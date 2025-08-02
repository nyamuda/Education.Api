using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Answers;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Answers;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Comments;
using Education.Api.Services.Abstractions.Flags;
using Education.Api.Services.Abstractions.Upvotes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswersController(
    IAnswerService answerService,
    IJwtService jwtService,
    IAnswerCommentService answerCommentService,
    IUpvoteService upvoteService,
    IAnswerFlagService answerFlagService
) : ControllerBase
{
    private readonly IAnswerService _answerService = answerService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IAnswerCommentService _answerCommentService = answerCommentService;
    private readonly IUpvoteService _upvoteService = upvoteService;
    private readonly IAnswerFlagService _answerFlagService = answerFlagService;

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

    //Upvotes an answer with a given ID
    [HttpPost("{answerId}/upvotes")]
    [Authorize]
    public async Task<IActionResult> Upvote(int answerId)
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

            await _upvoteService.UpvoteAnswerAsync(userId: userId, answerId: answerId);

            return StatusCode(201);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (ConflictException ex)
        {
            return StatusCode(409, ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Removes an upvote for an answer with a given ID
    [HttpDelete("{answerId}/upvotes")]
    [Authorize]
    public async Task<IActionResult> RemoveUpvote(int answerId)
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

            await _upvoteService.RemoveAnswerUpvoteAsync(userId: userId, answerId: answerId);

            return NoContent();
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

    //Flags an answer with a given ID
    [HttpPost("{answerId}/flags")]
    [Authorize]
    public async Task<IActionResult> Flag(int answerId, AddAnswerFlagDto dto)
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

            AnswerFlagDto answerFlag = await _answerFlagService.AddAsync(
                userId: userId,
                answerId: answerId,
                dto
            );

            return CreatedAtRoute(
                routeName: "GetAnswerFlagById",
                routeValues: new { id = answerFlag.Id },
                value: answerFlag
            );
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ErrorResponse.Create(ex.Message));
        }
        catch (ConflictException ex)
        {
            return StatusCode(409, ErrorResponse.Create(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }
}
