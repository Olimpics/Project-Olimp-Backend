using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DelegationOptionsController : ControllerBase
{
    private readonly IDelegationOptionsService _service;

    public DelegationOptionsController(IDelegationOptionsService service)
    {
        _service = service;
    }

    [HttpGet("faculties")]
    [RequirePermission(RbacPermissions.UsersUpdate)]
    public async Task<ActionResult<IReadOnlyList<AssignableFacultyDto>>> GetFaculties()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        return Ok(await _service.GetAssignableFacultiesAsync(userId.Value));
    }

    [HttpGet("departments")]
    [RequirePermission(RbacPermissions.UsersUpdate)]
    public async Task<ActionResult<IReadOnlyList<AssignableDepartmentDto>>> GetDepartments()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        return Ok(await _service.GetAssignableDepartmentsAsync(userId.Value));
    }

    [HttpGet("groups")]
    [RequirePermission(RbacPermissions.UsersUpdate)]
    public async Task<ActionResult<IReadOnlyList<AssignableGroupDto>>> GetGroups()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        return Ok(await _service.GetAssignableGroupsAsync(userId.Value));
    }

    [HttpGet("roles")]
    [RequirePermission(RbacPermissions.UsersUpdate)]
    public async Task<ActionResult<IReadOnlyList<AssignableRoleDto>>> GetRoles()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        return Ok(await _service.GetAssignableRolesAsync(userId.Value));
    }

    [HttpGet("permissions")]
    [RequirePermission(RbacPermissions.RolePermissionsUpdate)]
    public async Task<ActionResult<IReadOnlyList<AssignablePermissionDto>>> GetPermissions()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        return Ok(await _service.GetAssignablePermissionsAsync(userId.Value));
    }
}
