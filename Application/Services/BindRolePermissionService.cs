using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IBindRolePermissionService
{
    Task<IEnumerable<BindRolePermissionDto>> GetAllAsync();
    Task<BindRolePermissionDto?> GetByKeyAsync(Guid roleId, Guid permissionId);
    Task<(BindRolePermissionDto? dto, int? statusCode, string? errorMessage)> CreateAsync(Guid granterUserId, CreateBindRolePermissionDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(Guid granterUserId, Guid roleId, Guid permissionId, UpdateBindRolePermissionDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(Guid granterUserId, Guid roleId, Guid permissionId);
}

public class BindRolePermissionService : IBindRolePermissionService
{
    private readonly IBindRolePermissionRepository _repository;
    private readonly IRoleMaskService _roleMaskService;
    private readonly IPermissionDelegationService _delegation;

    public BindRolePermissionService(
        IBindRolePermissionRepository repository,
        IRoleMaskService roleMaskService,
        IPermissionDelegationService delegation)
    {
        _repository = repository;
        _roleMaskService = roleMaskService;
        _delegation = delegation;
    }

    public async Task<IEnumerable<BindRolePermissionDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<BindRolePermissionDto?> GetByKeyAsync(Guid roleId, Guid permissionId)
    {
        return await _repository.GetDtoAsync(roleId, permissionId);
    }

    public async Task<(BindRolePermissionDto? dto, int? statusCode, string? errorMessage)> CreateAsync(
        Guid granterUserId,
        CreateBindRolePermissionDto dto)
    {
        if (!await _repository.ExistsRoleAsync(dto.RoleId))
            return (null, StatusCodes.Status400BadRequest, "Role not found");

        if (!await _repository.ExistsPermissionAsync(dto.PermissionId))
            return (null, StatusCodes.Status400BadRequest, "Permission not found");

        if (!await _delegation.CanGrantAsync(granterUserId, dto.RoleId, dto.PermissionId))
            return (null, StatusCodes.Status403Forbidden, "You cannot grant this permission to this role.");

        if (await _repository.BindingExistsAsync(dto.RoleId, dto.PermissionId))
            return (null, StatusCodes.Status400BadRequest, "This role-permission binding already exists");

        var binding = new RolePermission
        {
            RoleId = dto.RoleId,
            PermissionId = dto.PermissionId
        };

        await _repository.AddAsync(binding);
        await _repository.SaveChangesAsync();
        await _roleMaskService.RecalculateRoleMaskAsync(dto.RoleId);

        var resultDto = await _repository.GetDtoAsync(dto.RoleId, dto.PermissionId);
        return (resultDto, null, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(
        Guid granterUserId,
        Guid roleId,
        Guid permissionId,
        UpdateBindRolePermissionDto dto)
    {
        var binding = await _repository.GetEntityAsync(roleId, permissionId);
        if (binding == null)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        if (!await _repository.ExistsRoleAsync(dto.RoleId))
            return (false, StatusCodes.Status400BadRequest, "Role not found");

        if (!await _repository.ExistsPermissionAsync(dto.PermissionId))
            return (false, StatusCodes.Status400BadRequest, "Permission not found");

        if (!await _delegation.CanGrantAsync(granterUserId, dto.RoleId, dto.PermissionId))
            return (false, StatusCodes.Status403Forbidden, "You cannot grant this permission to this role.");

        if ((dto.RoleId != roleId || dto.PermissionId != permissionId) &&
            await _repository.BindingExistsAsync(dto.RoleId, dto.PermissionId))
        {
            return (false, StatusCodes.Status400BadRequest, "This role-permission binding already exists");
        }

        await _repository.DeleteAsync(roleId, permissionId);
        await _repository.AddAsync(new RolePermission
        {
            RoleId = dto.RoleId,
            PermissionId = dto.PermissionId
        });
        await _repository.SaveChangesAsync();

        await _roleMaskService.RecalculateRoleMaskAsync(roleId);
        if (roleId != dto.RoleId)
            await _roleMaskService.RecalculateRoleMaskAsync(dto.RoleId);

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(
        Guid granterUserId,
        Guid roleId,
        Guid permissionId)
    {
        if (!await _delegation.CanGrantAsync(granterUserId, roleId, permissionId))
            return (false, StatusCodes.Status403Forbidden, "You cannot remove this permission from this role.");

        var deletedRows = await _repository.DeleteAsync(roleId, permissionId);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        await _roleMaskService.RecalculateRoleMaskAsync(roleId);
        return (true, StatusCodes.Status204NoContent, null);
    }
}
