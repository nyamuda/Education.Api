using Education.Api.Dtos.Answers;
using Education.Api.Dtos.Comments;
using Education.Api.Dtos.Flags;
using Education.Api.Dtos.Flags.Questions;
using Education.Api.Dtos.Questions;
using Education.Api.Enums.Questions;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Answers;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Comments;
using Education.Api.Services.Abstractions.Flags;
using Education.Api.Services.Abstractions.Questions;
using Education.Api.Services.Abstractions.Upvotes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController(
    IQuestionService questionService,
    IJwtService jwtService,
    IQuestionCommentService commentService,
    IAnswerService answerService,
    IUpvoteService upvoteService,
    IQuestionFlagService questionFlagService
) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IQuestionCommentService _questionCommentService = commentService;
    private readonly IAnswerService _answerService = answerService;
    private readonly IUpvoteService _upvoteService = upvoteService;
    private readonly IQuestionFlagService _questionFlagService = questionFlagService;

    //Gets a question by ID
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

    //Gets a paginated list of questions
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

    //Adds a new question
    [HttpPost]
    [Authorize]
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
            var message =
                "Something went wrong while saving your question. Double-check the details and try again.";
            return NotFound(ErrorResponse.Create(message: message, details: ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            var message =
                "Something went wrong while saving your question. Double-check the details and try again.";
            return BadRequest(ErrorResponse.Create(message: message, details: ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.ToString()));
        }
    }

    //Updates a question with a given ID
    [HttpPut("{id}")]
    [Authorize]
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

    //Updates the status of a question with a given ID
    [HttpPatch("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(int id, UpdateQuestionStatusDto statusDto)
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

            await _questionService.UpdateStatusAsync(userId: userId, questionId: id, statusDto);

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

    //Deletes a question with a given ID
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
    [Authorize]
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
    [HttpPost("{questionId}/answers")]
    [Authorize]
    public async Task<IActionResult> PostAnswer(int questionId, AddAnswerDto dto)
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

            AnswerDto answer = await _answerService.AddAsync(
                userId: userId,
                questionId: questionId,
                dto
            );

            return CreatedAtRoute(
                routeName: "GetAnswerById",
                routeValues: new { id = answer.Id },
                value: answer
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

    //Upvotes a question with a given ID
    [HttpPost("{questionId}/upvotes")]
    [Authorize]
    public async Task<IActionResult> Upvote(int questionId)
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

            await _upvoteService.UpvoteQuestionAsync(userId: userId, questionId: questionId);

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

    //Removes an upvote for a question with a given ID
    [HttpDelete("{questionId}/upvotes")]
    [Authorize]
    public async Task<IActionResult> RemoveUpvote(int questionId)
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

            await _upvoteService.RemoveQuestionUpvoteAsync(userId: userId, questionId: questionId);

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

    //Flags a question with a given ID
    [HttpPost("{questionId}/flags")]
    [Authorize]
    public async Task<IActionResult> Flag(int questionId, AddQuestionFlagDto dto)
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

            QuestionFlagDto questionFlag = await _questionFlagService.AddAsync(
                userId: userId,
                questionId: questionId,
                dto
            );

            return CreatedAtRoute(
                routeName: "GetQuestionFlagById",
                routeValues: new { id = questionFlag.Id },
                value: questionFlag
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
