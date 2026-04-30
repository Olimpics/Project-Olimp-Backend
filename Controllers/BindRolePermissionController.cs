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

    [HttpGet("{roleId:int}/{permissionId:int}")]
    [RequirePermission(RbacPermissions.RolePermissionsRead)]
    public async Task<ActionResult<BindRolePermissionDto>> GetBindRolePermission(int roleId, int permissionId)
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

    [HttpPut("{roleId:int}/{permissionId:int}")]
    [RequirePermission(RbacPermissions.RolePermissionsUpdate)]
    public async Task<IActionResult> UpdateBindRolePermission(int roleId, int permissionId, UpdateBindRolePermissionDto dto)
    {
        var (success, statusCode, errorMessage) = await _service.UpdateAsync(roleId, permissionId, dto);

        if (!success)
            return StatusCode(statusCode, errorMessage);

        return NoContent();
    }

    [HttpDelete("{roleId:int}/{permissionId:int}")]
    [RequirePermission(RbacPermissions.RolePermissionsDelete)]
    public async Task<IActionResult> DeleteBindRolePermission(int roleId, int permissionId)
    {
        var (success, statusCode, errorMessage) = await _service.DeleteAsync(roleId, permissionId);

        if (!success)
            return StatusCode(statusCode, errorMessage);

        return NoContent();
    }
}
