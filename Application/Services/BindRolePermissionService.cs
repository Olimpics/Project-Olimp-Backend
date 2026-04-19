using AutoMapper;
using Microsoft.AspNetCore.Http;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IBindRolePermissionService
{
    Task<IEnumerable<BindRolePermissionDto>> GetAllAsync();
    Task<BindRolePermissionDto?> GetByIdAsync(int id);
    Task<(BindRolePermissionDto? dto, int? statusCode, string? errorMessage)> CreateAsync(CreateBindRolePermissionDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateBindRolePermissionDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
}

public class BindRolePermissionService : IBindRolePermissionService
{
    private readonly IBindRolePermissionRepository _repository;
    private readonly IRoleMaskService _roleMaskService;
    private readonly IMapper _mapper;

    public BindRolePermissionService(IBindRolePermissionRepository repository, IRoleMaskService roleMaskService, IMapper mapper)
    {
        _repository = repository;
        _roleMaskService = roleMaskService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BindRolePermissionDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<BindRolePermissionDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<(BindRolePermissionDto? dto, int? statusCode, string? errorMessage)> CreateAsync(CreateBindRolePermissionDto dto)
    {
        if (!await _repository.ExistsRoleAsync(dto.RoleId))
            return (null, StatusCodes.Status400BadRequest, "Role not found");

        if (!await _repository.ExistsPermissionAsync(dto.PermissionId))
            return (null, StatusCodes.Status400BadRequest, "Permission not found");

        if (await _repository.BindingExistsAsync(dto.RoleId, dto.PermissionId))
            return (null, StatusCodes.Status400BadRequest, "This role-permission binding already exists");

        var binding = _mapper.Map<BindRolePermission>(dto);

        await _repository.AddAsync(binding);
        await _repository.SaveChangesAsync();
        await _roleMaskService.RecalculateRoleMaskAsync(binding.RoleId);

        var resultDto = await _repository.GetDtoByIdAsync(binding.IdBindRolePermission);
        return (resultDto, null, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateBindRolePermissionDto dto)
    {
        if (id != dto.IdBindRolePermission)
            return (false, StatusCodes.Status400BadRequest, "ID mismatch");

        var binding = await _repository.GetEntityByIdAsync(id);
        if (binding == null)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        var oldRoleId = binding.RoleId;

        if (!await _repository.ExistsRoleAsync(dto.RoleId))
            return (false, StatusCodes.Status400BadRequest, "Role not found");

        if (!await _repository.ExistsPermissionAsync(dto.PermissionId))
            return (false, StatusCodes.Status400BadRequest, "Permission not found");

        if (await _repository.BindingExistsAsync(dto.RoleId, dto.PermissionId, id))
            return (false, StatusCodes.Status400BadRequest, "This role-permission binding already exists");

        _mapper.Map(dto, binding);
        await _repository.SaveChangesAsync();
        await _roleMaskService.RecalculateRoleMaskAsync(dto.RoleId);
        if (oldRoleId != dto.RoleId)
            await _roleMaskService.RecalculateRoleMaskAsync(oldRoleId);

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
    {
        var binding = await _repository.GetEntityByIdAsync(id);
        if (binding == null)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        var deletedRows = await _repository.DeleteAsync(id);

        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        await _roleMaskService.RecalculateRoleMaskAsync(binding.RoleId);

        return (true, StatusCodes.Status204NoContent, null);
    }
}