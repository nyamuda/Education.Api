using Education.Api.Dtos.Curriculums;
using Education.Api.Enums.Curriculums;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Curriculums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CurriculumsController(ICurriculumService curriculumService) : ControllerBase
{
    private readonly ICurriculumService _curriculumService = curriculumService;

    //Gets a curriculum by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var curriculum = await _curriculumService.GetByIdAsync(id);
            return Ok(curriculum);
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

    //Gets a paginated list of curriculums
    [HttpGet]
    public async Task<IActionResult> Get(
        int page = 1,
        int pageSize = 10,
        CurriculumSortOption sortBy = CurriculumSortOption.DateCreated
    )
    {
        try
        {
            var curriculums = await _curriculumService.GetAsync(
                page: page,
                pageSize: pageSize,
                sortBy
            );
            // var curriculums = await _curriculumService.DeserializeCurriculumsFromFileAsync();
            return Ok(curriculums);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Adds a new curriculum
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(AddCurriculumDto dto)
    {
        try
        {
            CurriculumDto curriculum = await _curriculumService.AddAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = curriculum.Id }, curriculum);
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

    //Updates a curriculum with a given ID
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, UpdateCurriculumDto dto)
    {
        try
        {
            await _curriculumService.UpdateAsync(id, dto);

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

    //Deletes a curriculum with a given ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _curriculumService.DeleteAsync(id);

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
