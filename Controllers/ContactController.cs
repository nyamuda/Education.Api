using Education.Api.Dtos.Contact;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Contact;
using Microsoft.AspNetCore.Mvc;

namespace Education.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactController(IContactService contactService) : ControllerBase
{
    private readonly IContactService _contactService = contactService;

    [HttpPost]
    public async Task<IActionResult> Post(ContactDto dto)
    {
        try
        {
            await _contactService.SendAsync(dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.Unexpected(details: ex.Message));
        }
    }
}
