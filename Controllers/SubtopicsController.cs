using Education.Api.Dtos.Topics.Subtopics;
using Education.Api.Enums.Subtopics;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Topics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubtopicsController(ISubtopicService subtopicService) : ControllerBase
{
    private readonly ISubtopicService _subtopicService = subtopicService;

    //Gets a subtopic by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var subtopic = await _subtopicService.GetByIdAsync(id);
            return Ok(subtopic);
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

    //Gets a paginated list of subtopics
    [HttpGet]
    public async Task<IActionResult> Get(
        int? curriculumId,
        int? examBoardId,
        int? levelId,
        int? subjectId,
        int? topicId,
        int page = 1,
        int pageSize = 10,
        SubtopicSortOption sortBy = SubtopicSortOption.DateCreated
    )
    {
        try
        {
            SubtopicQueryParams queryParams =
                new()
                {
                    CurriculumId = curriculumId,
                    ExamBoardId = examBoardId,
                    LevelId = levelId,
                    SubjectId = subjectId,
                    TopicId = topicId,
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy
                };
            var subtopics = await _subtopicService.GetAsync(queryParams);
            return Ok(subtopics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Adds a new subtopic
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(AddSubtopicDto dto)
    {
        try
        {
            SubtopicDto subtopic = await _subtopicService.AddAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = subtopic.Id }, subtopic);
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

    //Updates a subtopic with a given ID
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, UpdateSubtopicDto dto)
    {
        try
        {
            await _subtopicService.UpdateAsync(id, dto);

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

    //Deletes a subtopic with a given ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _subtopicService.DeleteAsync(id);

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
