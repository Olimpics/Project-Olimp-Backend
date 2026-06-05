using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO.Sg;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SgController : ControllerBase
{
    private readonly ISgService _sgService;

    public SgController(ISgService sgService)
    {
        _sgService = sgService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // 1. Get user's subdivisions
    [HttpGet("user-subdivisions")]
    public async Task<ActionResult<List<SubDivisionUserDto>>> GetUserSubDivisions()
    {
        return await _sgService.GetUserSubDivisionsAsync(CurrentUserId);
    }

    // 2. Get events
    [HttpGet("events")]
    public async Task<ActionResult<List<EventDto>>> GetEvents(
        [FromQuery] Guid subDivisionId, 
        [FromQuery] Guid? facultyId, 
        [FromQuery] Guid catalogYearId, 
        [FromQuery] bool isEven, 
        [FromQuery] string? search)
    {
        return await _sgService.GetEventsAsync(subDivisionId, facultyId, catalogYearId, isEven, search);
    }

    // 3. Get students in SG
    [HttpGet("students")]
    public async Task<ActionResult<List<StudentSgDto>>> GetStudentsInSg(
        [FromQuery] Guid subDivisionId, 
        [FromQuery] Guid? facultyId, 
        [FromQuery] string? search)
    {
        return await _sgService.GetStudentsInSgAsync(subDivisionId, facultyId, search);
    }

    // 4-6. Event CRUD
    [HttpPost("events")]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] EventCreateUpdateDto dto)
    {
        var result = await _sgService.CreateEventAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetEventById), new { id = result.IdEvent }, result);
    }

    [HttpGet("events/{id}")]
    public async Task<ActionResult<EventDto>> GetEventById(Guid id)
    {
        var result = await _sgService.GetEventByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("events/{id}")]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventCreateUpdateDto dto)
    {
        if (await _sgService.UpdateEventAsync(id, dto)) return NoContent();
        return NotFound();
    }

    [HttpDelete("events/{id}")]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        if (await _sgService.DeleteEventAsync(id)) return NoContent();
        return NotFound();
    }

    // 7-9. BindEventStudent CRUD
    [HttpGet("events/{eventId}/students")]
    public async Task<ActionResult<List<BindEventStudentDto>>> GetEventStudents(Guid eventId, [FromQuery] string? search)
    {
        return await _sgService.GetEventStudentsAsync(eventId, search);
    }

    [HttpPost("event-students")]
    public async Task<IActionResult> AddStudentToEvent([FromBody] BindEventStudentCreateUpdateDto dto)
    {
        if (await _sgService.AddStudentToEventAsync(dto)) return Ok();
        return BadRequest();
    }

    [HttpPut("event-students/{id}")]
    public async Task<IActionResult> UpdateStudentInEvent(Guid id, [FromBody] BindEventStudentCreateUpdateDto dto)
    {
        if (await _sgService.UpdateStudentInEventAsync(id, dto)) return NoContent();
        return NotFound();
    }

    [HttpDelete("event-students/{id}")]
    public async Task<IActionResult> DeleteStudentFromEvent(Guid id)
    {
        if (await _sgService.DeleteStudentFromEventAsync(id)) return NoContent();
        return NotFound();
    }

    // 10-12. MembersOfSg CRUD
    [HttpPost("members")]
    public async Task<IActionResult> AddStudentToSubDivision([FromBody] MemberSgCreateUpdateDto dto)
    {
        if (await _sgService.AddStudentToSubDivisionAsync(dto)) return Ok();
        return BadRequest();
    }

    [HttpPut("members/{id}")]
    public async Task<IActionResult> UpdateMemberSg(Guid id, [FromBody] MemberSgCreateUpdateDto dto)
    {
        if (await _sgService.UpdateMemberSgAsync(id, dto)) return NoContent();
        return NotFound();
    }

    [HttpDelete("members/{id}")]
    public async Task<IActionResult> DeleteMemberSg(Guid id)
    {
        if (await _sgService.DeleteMemberSgAsync(id)) return NoContent();
        return NotFound();
    }

    // 13-15. InventorySg CRUD
    [HttpGet("inventory")]
    public async Task<ActionResult<List<InventorySgDto>>> GetInventory([FromQuery] string? search)
    {
        return await _sgService.GetInventoryAsync(search);
    }

    [HttpPost("inventory")]
    public async Task<IActionResult> AddInventoryItem([FromBody] InventorySgCreateUpdateDto dto)
    {
        if (await _sgService.AddInventoryItemAsync(dto)) return Ok();
        return BadRequest();
    }

    [HttpPut("inventory/{id}")]
    public async Task<IActionResult> UpdateInventoryItem(Guid id, [FromBody] InventorySgCreateUpdateDto dto)
    {
        if (await _sgService.UpdateInventoryItemAsync(id, dto)) return NoContent();
        return NotFound();
    }

    [HttpDelete("inventory/{id}")]
    public async Task<IActionResult> DeleteInventoryItem(Guid id)
    {
        if (await _sgService.DeleteInventoryItemAsync(id)) return NoContent();
        return NotFound();
    }

    // 16-18. AccountingJournal CRUD
    [HttpGet("inventory/{inventoryId}/journal")]
    public async Task<ActionResult<List<AccountingJournalDto>>> GetJournalEntries(Guid inventoryId, [FromQuery] string? search)
    {
        return await _sgService.GetJournalEntriesAsync(inventoryId, search);
    }

    [HttpPost("journal")]
    public async Task<IActionResult> AddJournalEntry([FromBody] AccountingJournalCreateUpdateDto dto)
    {
        try
        {
            if (await _sgService.AddJournalEntryAsync(dto)) return Ok();
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("journal/{id}")]
    public async Task<IActionResult> UpdateJournalEntry(Guid id, [FromBody] AccountingJournalCreateUpdateDto dto)
    {
        try
        {
            if (await _sgService.UpdateJournalEntryAsync(id, dto)) return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("journal/{id}")]
    public async Task<IActionResult> DeleteJournalEntry(Guid id)
    {
        if (await _sgService.DeleteJournalEntryAsync(id)) return NoContent();
        return NotFound();
    }

    [HttpPost("journal/{id}/confirm-return")]
    public async Task<IActionResult> ConfirmReturn(Guid id)
    {
        try
        {
            if (await _sgService.ConfirmReturnAsync(id, CurrentUserId)) return Ok();
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
