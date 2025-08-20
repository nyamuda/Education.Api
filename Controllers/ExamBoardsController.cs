using Education.Api.Dtos.ExamBoards;
using Education.Api.Dtos.Levels;
using Education.Api.Enums.ExamBoards;
using Education.Api.Enums.Levels;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.ExamBoards;
using Education.Api.Services.Abstractions.Levels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/exam-boards")]
[ApiController]
public class ExamBoardsController(IExamBoardService examBoardService, ILevelService levelService)
    : ControllerBase
{
    private readonly IExamBoardService _examBoardService = examBoardService;
    private readonly ILevelService _levelService = levelService;

    //Gets an exam board by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var examBoard = await _examBoardService.GetByIdAsync(id);
            return Ok(examBoard);
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

    //Gets a paginated list of exam boards
    [HttpGet]
    public async Task<IActionResult> Get(
        int? curriculumId,
        int page = 1,
        int pageSize = 10,
        ExamBoardSortOption sortBy = ExamBoardSortOption.DateCreated
    )
    {
        try
        {
            var examBoards = await _examBoardService.GetAsync(
                curriculumId: curriculumId,
                page: page,
                pageSize: pageSize,
                sortBy: sortBy
            );
            return Ok(examBoards);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Adds a new exam board
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(AddExamBoardDto dto)
    {
        try
        {
            ExamBoardDto examBoard = await _examBoardService.AddAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = examBoard.Id }, examBoard);
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

    //Updates an exam board with a given ID
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, UpdateExamBoardDto dto)
    {
        try
        {
            await _examBoardService.UpdateAsync(id, dto);

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

    //Deletes an exam board with a given ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _examBoardService.DeleteAsync(id);

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

    //Gets a paginated list of levels for a specific exam board
    [HttpGet("{examBoardId}/levels")]
    public async Task<IActionResult> GetLevels(
        int? curriculumId,
        int examBoardId,
        int page = 1,
        int pageSize = 10,
        LevelSortOption sortBy = LevelSortOption.DateCreated
    )
    {
        try
        {
            PageInfo<LevelDto> levels = await _levelService.GetAsync(
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

    //Adds a new level for a specific exam board
    [HttpPost("{examBoardId}/levels")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PostLevel(int examBoardId, AddLevelDto dto)
    {
        try
        {
            LevelDto level = await _levelService.AddAsync(examBoardId, dto);

            return CreatedAtRoute(
                routeName: "GetLevelById",
                routeValues: new { id = level.Id },
                level
            );
        }
        catch (ConflictException ex)
        {
            return StatusCode(409, ErrorResponse.Create(ex.Message));
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

    //Updates a level for a specific exam board
    [HttpPut("{examBoardId}/levels/{levelId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLevel(int examBoardId, int levelId, UpdateLevelDto dto)
    {
        try
        {
            await _levelService.UpdateAsync(examBoardId: examBoardId, levelId: levelId, dto);

            return NoContent();
        }
        catch (ConflictException ex)
        {
            return StatusCode(409, ErrorResponse.Create(ex.Message));
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
