using Education.Api.Dtos.Flags.Answers;
using Education.Api.Dtos.Flags.Comments;
using Education.Api.Dtos.Flags.Questions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Flags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class FlagsController(
    IQuestionFlagService questionFlagService,
    IAnswerFlagService answerFlagService,
    ICommentFlagService commentFlagService
) : ControllerBase
{
    private readonly IQuestionFlagService _questionFlagService = questionFlagService;
    private readonly IAnswerFlagService _answerFlagService = answerFlagService;
    private readonly ICommentFlagService _commentFlagService = commentFlagService;

    //Gets a question flag by ID
    [HttpGet("questions/{id}", Name = "GetQuestionFlagById")]
    public async Task<IActionResult> GetQuestionFlag(int id)
    {
        try
        {
            QuestionFlagDto questionFlag = await _questionFlagService.GetByIdAsync(id);
            return Ok(questionFlag);
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

    //Gets a paginated list of question flags
    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestionFlags(int page = 1, int pageSize = 10)
    {
        try
        {
            PageInfo<QuestionFlagDto> flags = await _questionFlagService.GetAsync(
                page: page,
                pageSize: pageSize
            );
            return Ok(flags);
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

    //Deletes a question flag with a given ID
    [HttpDelete("questions/{id}")]
    public async Task<IActionResult> DeleteQuestionFlag(int id)
    {
        try
        {
            await _questionFlagService.DeleteAsync(id);
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

    //Gets an answer flag by ID
    [HttpGet("answers/{id}", Name = "GetAnswerFlagById")]
    public async Task<IActionResult> GetAnswerFlag(int id)
    {
        try
        {
            AnswerFlagDto answerFlag = await _answerFlagService.GetByIdAsync(id);
            return Ok(answerFlag);
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

    //Gets a paginated list of answer flags
    [HttpGet("answers")]
    public async Task<IActionResult> GetAnswerFlags(int page = 1, int pageSize = 10)
    {
        try
        {
            PageInfo<AnswerFlagDto> flags = await _answerFlagService.GetAsync(
                page: page,
                pageSize: pageSize
            );
            return Ok(flags);
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

    //Deletes an answer flag with a given ID
    [HttpDelete("answers/{id}")]
    public async Task<IActionResult> DeleteAnswerFlag(int id)
    {
        try
        {
            await _answerFlagService.DeleteAsync(id);
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

    //Gets a comment flag by ID
    [HttpGet("comments/{id}", Name = "GetCommentFlagById")]
    public async Task<IActionResult> GetCommentFlag(int id)
    {
        try
        {
            CommentFlagDto commentFlag = await _commentFlagService.GetByIdAsync(id);
            return Ok(commentFlag);
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

    //Gets a paginated list of comment flags
    [HttpGet("comments")]
    public async Task<IActionResult> GetCommentFlags(int page = 1, int pageSize = 10)
    {
        try
        {
            PageInfo<CommentFlagDto> flags = await _commentFlagService.GetAsync(
                page: page,
                pageSize: pageSize
            );
            return Ok(flags);
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

    //Deletes a comment flag with a given ID
    [HttpDelete("comments/{id}")]
    public async Task<IActionResult> DeleteCommentFlag(int id)
    {
        try
        {
            await _commentFlagService.DeleteAsync(id);
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
}
