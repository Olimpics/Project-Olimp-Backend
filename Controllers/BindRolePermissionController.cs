using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BindRolePermissionController : ControllerBase
{
    private readonly IBindRolePermissionService _service;

    public BindRolePermissionController(IBindRolePermissionService service)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermission(RbacPermissions.RolePermissionsRead)]
    public async Task<ActionResult<IEnumerable<BindRolePermissionDto>>> GetBindRolePermissions()
    {
        var bindings = await _service.GetAllAsync();
        return Ok(bindings);
    }

    [HttpGet("{roleId:guid}/{permissionId:guid}")]
    [RequirePermission(RbacPermissions.RolePermissionsRead)]
    public async Task<ActionResult<BindRolePermissionDto>> GetBindRolePermission(Guid roleId, Guid permissionId)
    {
        var binding = await _service.GetByKeyAsync(roleId, permissionId);
        if (binding == null)
            return NotFound();

        return Ok(binding);
    }

    [HttpPost]
    [RequirePermission(RbacPermissions.RolePermissionsCreate)]
    public async Task<ActionResult<BindRolePermissionDto>> CreateBindRolePermission(CreateBindRolePermissionDto dto)
    {
        var (resultDto, statusCode, errorMessage) = await _service.CreateAsync(dto);

        if (statusCode.HasValue)
            return StatusCode(statusCode.Value, errorMessage);

        return CreatedAtAction(nameof(GetBindRolePermission),
            new { roleId = resultDto!.RoleId, permissionId = resultDto.PermissionId },
            resultDto);
    }

    [HttpPut("{roleId:guid}/{permissionId:guid}")]
    [RequirePermission(RbacPermissions.RolePermissionsUpdate)]
    public async Task<IActionResult> UpdateBindRolePermission(Guid roleId, Guid permissionId, UpdateBindRolePermissionDto dto)
    {
        var (success, statusCode, errorMessage) = await _service.UpdateAsync(roleId, permissionId, dto);

        if (!success)
            return StatusCode(statusCode, errorMessage);

        return NoContent();
    }

    [HttpDelete("{roleId:int}/{permissionId:int}")]
    [RequirePermission(RbacPermissions.RolePermissionsDelete)]
    public async Task<IActionResult> DeleteBindRolePermission(Guid roleId, Guid permissionId)
    {
        var (success, statusCode, errorMessage) = await _service.DeleteAsync(roleId, permissionId);

        if (!success)
            return StatusCode(statusCode, errorMessage);

        return NoContent();
    }
}
