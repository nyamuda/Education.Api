using Education.Api.Dtos.Levels;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Levels;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LevelsController(ILevelService levelService) : ControllerBase
{
    private readonly ILevelService _levelService = levelService;

    //Gets a level with a given ID
    [HttpGet("{id}")]
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
}
