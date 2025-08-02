using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Comments;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Comments;
using Education.Api.Services.Abstractions.Flags;
using Education.Api.Services.Abstractions.Upvotes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController(
    ICommentService commentService,
    IJwtService jwtService,
    IUpvoteService upvoteService,
    ICommentFlagService commentFlagService
) : ControllerBase
{
    private readonly ICommentService _commentService = commentService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUpvoteService _upvoteService = upvoteService;
    private readonly ICommentFlagService _commentFlagService = commentFlagService;

    //Gets a comment with a given ID
    [HttpGet("{id}", Name = "GetCommentById")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            CommentDto comment = await _commentService.GetByIdAsync(id);
            return Ok(comment);
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

    //Updates a comment with a given ID
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id, UpdateCommentDto dto)
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

            await _commentService.UpdateAsync(userId: userId, commentId: id, dto);

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

    //Deletes a comment with a given ID
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

            await _commentService.DeleteAsync(userId: userId, commentId: id);

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

    //Upvotes a comment with a given ID
    [HttpPost("{commentId}/upvotes")]
    [Authorize]
    public async Task<IActionResult> Upvote(int commentId)
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

            await _upvoteService.UpvoteCommentAsync(userId: userId, commentId: commentId);

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

    //Removes an upvote for a comment with a given ID
    [HttpDelete("{commentId}/upvotes")]
    [Authorize]
    public async Task<IActionResult> RemoveUpvote(int commentId)
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

            await _upvoteService.RemoveCommentUpvoteAsync(userId: userId, commentId: commentId);

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

    //Flags a comment with a given ID
    [HttpPost("{commentId}/flags")]
    [Authorize]
    public async Task<IActionResult> Flag(int commentId, AddCommentFlagDto dto)
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

            CommentFlagDto commentFlag = await _commentFlagService.AddAsync(
                userId: userId,
                commentId: commentId,
                dto
            );

            return CreatedAtRoute(
                routeName: "GetCommentFlagById",
                routeValues: new { id = commentFlag.Id },
                value: commentFlag
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
