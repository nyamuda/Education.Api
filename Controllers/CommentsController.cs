using Education.Api.Dtos.Comments;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Comments;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController(ICommentService commentService, IJwtService jwtService)
    : ControllerBase
{
    private readonly ICommentService _commentService = commentService;
    private readonly IJwtService _jwtService = jwtService;

    [HttpGet("{id}")]
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
}
