using Education.Api.Dtos.Flags.Questions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Flags;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
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
    
     //Deletes a question flag with a given ID
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
}
