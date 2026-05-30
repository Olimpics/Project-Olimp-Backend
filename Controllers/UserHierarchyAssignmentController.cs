using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserHierarchyAssignmentController : ControllerBase
{
    private readonly IUserHierarchyAssignmentService _service;

    public UserHierarchyAssignmentController(IUserHierarchyAssignmentService service)
    {
        _service = service;
    }

    [HttpGet("by-user/{userId:guid}")]
    [RequirePermission(RbacPermissions.UsersRead)]
    public async Task<ActionResult<IReadOnlyList<UserHierarchyAssignmentDto>>> GetByUser(Guid userId)
    {
        var assignments = await _service.GetByUserIdAsync(userId);
        return Ok(assignments);
    }

    [HttpGet("{id:guid}")]
    [RequirePermission(RbacPermissions.UsersRead)]
    public async Task<ActionResult<UserHierarchyAssignmentDto>> GetById(Guid id)
    {
        var assignment = await _service.GetByIdAsync(id);
        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }

    [HttpPost]
    [RequirePermission(RbacPermissions.UsersUpdate)]
    public async Task<ActionResult<UserHierarchyAssignmentDto>> Create(CreateUserHierarchyAssignmentDto dto)
    {
        var granterUserId = User.GetUserId();
        if (!granterUserId.HasValue)
            return Unauthorized();

        var (result, statusCode, errorMessage) = await _service.CreateAsync(granterUserId.Value, dto);

        if (statusCode.HasValue)
            return StatusCode(statusCode.Value, errorMessage);

        return CreatedAtAction(nameof(GetById), new { id = result!.IdAssignment }, result);
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(RbacPermissions.UsersUpdate)]
    public async Task<IActionResult> Update(Guid id, UpdateUserHierarchyAssignmentDto dto)
    {
        var granterUserId = User.GetUserId();
        if (!granterUserId.HasValue)
            return Unauthorized();

        var (success, statusCode, errorMessage) = await _service.UpdateAsync(granterUserId.Value, id, dto);

        if (!success)
            return StatusCode(statusCode, errorMessage);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(RbacPermissions.UsersUpdate)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var granterUserId = User.GetUserId();
        if (!granterUserId.HasValue)
            return Unauthorized();

        var (success, statusCode, errorMessage) = await _service.DeleteAsync(granterUserId.Value, id);

        if (!success)
            return StatusCode(statusCode, errorMessage);

        return NoContent();
    }
}
