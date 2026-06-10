using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface IApprovalService
{
    Task<IEnumerable<ApprovalDto>> GetAllAsync();
    Task<ApprovalDto?> GetByIdAsync(Guid id);
    Task<ApprovalDto> CreateAsync(CreateApprovalDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(Guid id, UpdateApprovalDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(Guid id);
}

public class ApprovalService : IApprovalService
{
    private readonly IApprovalRepository _repository;
    private readonly IMapper _mapper;

    public ApprovalService(IApprovalRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ApprovalDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<ApprovalDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<ApprovalDto> CreateAsync(CreateApprovalDto dto)
    {
        var entity = _mapper.Map<Approval>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return await _repository.GetDtoByIdAsync(entity.IdApproval) ?? _mapper.Map<ApprovalDto>(entity);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(Guid id, UpdateApprovalDto dto)
    {
        if (id != dto.IdApproval)
            return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

        var entity = await _repository.GetEntityByIdAsync(id);
        if (entity == null)
            return (false, StatusCodes.Status404NotFound, "Approval not found.");

        _mapper.Map(dto, entity);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Approval not found.");
            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(Guid id)
    {
        var deletedRows = await _repository.DeleteAsync(id);
        if (deletedRows == 0)
            return (false, StatusCodes.Status404NotFound, "Approval not found.");
        return (true, StatusCodes.Status204NoContent, null);
    }
}
