using Azure;
using Education.Api.Dtos.Levels;
using Education.Api.Enums.Levels;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Levels;
using Education.Api.Services.Abstractions.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LevelsController(ILevelService levelService, ISubjectService subjectService)
    : ControllerBase
{
    private readonly ILevelService _levelService = levelService;
    private readonly ISubjectService _subjectService = subjectService;

    //Gets a level with a given ID
    [HttpGet("{id}", Name = "GetLevelById")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            LevelDto level = await _levelService.GetByIdAsync(id);
            return Ok(level);
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

    //Gets all levels
    [HttpGet]
    public async Task<IActionResult> Get(
        int? curriculumId,
        int? examBoardId,
        int page = 1,
        int pageSize = 10,
        LevelSortOption sortBy = LevelSortOption.DateCreated
    )
    {
        try
        {
            var levels = await _levelService.GetAsync(
                curriculumId: curriculumId,
                examBoardId: examBoardId,
                page: page,
                pageSize: pageSize,
                sortBy: sortBy
            );
            return Ok(levels);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Deletes a level with a given ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _levelService.DeleteAsync(id);
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

    //Gets subjects for a level with a given ID
    [HttpGet("{levelId}/subjects")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSubjects(int levelId, int page = 1, int pageSize = 10)
    {
        try
        {
            SubjectQueryParams queryParams =
                new()
                {
                    LevelId = levelId,
                    Page = page,
                    PageSize = pageSize
                };
            var subjects = await _subjectService.GetForLevelAsync(queryParams);
            return Ok(subjects);
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
