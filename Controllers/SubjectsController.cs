using Azure;
using Education.Api.Dtos.Subjects;
using Education.Api.Enums.Subjects;
using Education.Api.Exceptions;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectsController(ISubjectService subjectService) : ControllerBase
{
    private readonly ISubjectService _subjectService = subjectService;

    //Gets a subject with a given ID
    [HttpGet("{id}", Name = "GetSubjectById")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            SubjectDto subject = await _subjectService.GetByIdAsync(id);
            return Ok(subject);
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

    //Gets all subjects
    [HttpGet]
    public async Task<IActionResult> Get(
        int? curriculumId,
        int? examBoardId,
        int? levelId,
        int page = 1,
        int pageSize = 10,
        SubjectSortOption sortBy = SubjectSortOption.DateCreated
    )
    {
        try
        {
            SubjectQueryParams queryParams =
                new()
                {
                    CurriculumId = curriculumId,
                    ExamBoardId = examBoardId,
                    LevelId = levelId,
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy
                };
            var subjects = await _subjectService.GetAsync(queryParams);
            return Ok(subjects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(ex.Message));
        }
    }

    //Adds a new subject
    [HttpPost]
    public async Task<IActionResult> Post(AddSubjectDto dto)
    {
        try
        {
            SubjectDto subject = await _subjectService.AddAsync(dto);
            return CreatedAtRoute(
                routeName: "GetSubjectById",
                routeValues: new { id = subject.Id },
                value: subject
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

    //Deletes a subject with a given ID
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _subjectService.DeleteAsync(id);
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
