using Education.Api.Dtos.Topics;
using Education.Api.Enums.Topics;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Topics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TopicsController(ITopicService topicService) : ControllerBase
{
    private readonly ITopicService _topicService = topicService;

    //Gets a topic by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var topic = await _topicService.GetByIdAsync(id);
            return Ok(topic);
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

    //Gets a paginated list of topics
    [HttpGet]
    public async Task<IActionResult> Get(
        int? curriculumId,
        int? examBoardId,
        int? levelId,
        int? subjectId,
        int page = 1,
        int pageSize = 10,
        TopicSortOption sortBy = TopicSortOption.DateCreated
    )
    {
        try
        {
            TopicQueryParams queryParams =
                new()
                {
                    CurriculumId = curriculumId,
                    ExamBoardId = examBoardId,
                    LevelId = levelId,
                    SubjectId = subjectId,
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy
                };
            var topics = await _topicService.GetAsync(queryParams);

            return Ok(topics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Adds a new topic
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(AddTopicDto dto)
    {
        try
        {
            TopicDto topic = await _topicService.AddAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = topic.Id }, topic);
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

    //Updates a topic with a given ID
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, UpdateTopicDto dto)
    {
        try
        {
            await _topicService.UpdateAsync(id, dto);

            return NoContent();
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

    //Deletes a topic with a given ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _topicService.DeleteAsync(id);

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
