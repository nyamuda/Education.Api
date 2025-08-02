using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Questions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Answers;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Comments;
using Education.Api.Services.Abstractions.Questions;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController(
    IQuestionService questionService,
    IJwtService jwtService,
    IQuestionCommentService commentService,
    IAnswerService answerService
) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IQuestionCommentService _questionCommentService = commentService;
    private readonly IAnswerService _answerService = answerService;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var question = await _questionService.GetByIdAsync(id);
            return Ok(question);
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

    [HttpGet]
    public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
    {
        try
        {
            var questions = await _questionService.GetAsync(page: page, pageSize: pageSize);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(AddQuestionDto dto)
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

            QuestionDto question = await _questionService.AddAsync(userId, dto);

            return CreatedAtAction(nameof(Get), new { id = question.Id }, question);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UpdateQuestionDto dto)
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

            await _questionService.UpdateAsync(userId: userId, questionId: id, dto);

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

            await _questionService.DeleteAsync(userId: userId, questionId: id);

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

    //Gets a paginated list of comments for a given question
    [HttpGet("{questionId}/comments")]
    public async Task<IActionResult> GetComments(int questionId, int page = 1, int pageSize = 10)
    {
        try
        {
            PageInfo<CommentDto> comments = await _questionCommentService.GetAsync(
                questionId: questionId,
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

    //Adds a new comment for a question with a given ID
    [HttpPost("{questionId}/comments")]
    public async Task<IActionResult> PostComment(int questionId, AddCommentDto dto)
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

            CommentDto comment = await _questionCommentService.AddAsync(
                userId: userId,
                questionId: questionId,
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

    //Gets a paginated list of answers for a given question
    [HttpGet("{questionId}/answers")]
    public async Task<IActionResult> GetAnswers(int questionId, int page = 1, int pageSize = 10)
    {
        try
        {
            PageInfo<AnswerDto> answers = await _answerService.GetAsync(
                questionId: questionId,
                page: page,
                pageSize: pageSize
            );

            return Ok(answers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Adds a new answer for a question with a given ID
    [HttpPost("{questionId}/comments")]
    public async Task<IActionResult> PostAnswer(int questionId, AddCommentDto dto)
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

            CommentDto comment = await _questionCommentService.AddAsync(
                userId: userId,
                questionId: questionId,
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
