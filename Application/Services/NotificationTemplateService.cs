using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public interface INotificationTemplateService
{
    Task<IEnumerable<NotificationTemplateDto>> GetAllAsync();
    Task<NotificationTemplateDto?> GetByIdAsync(int id);
    Task<NotificationTemplateDto> CreateAsync(CreateNotificationTemplateDto dto);
    Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateNotificationTemplateDto dto);
}

public class NotificationTemplateService : INotificationTemplateService
{
    private readonly INotificationTemplateRepository _repository;
    private readonly IMapper _mapper;

    public NotificationTemplateService(INotificationTemplateRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetAllAsync()
    {
        return await _repository.GetAllDtoAsync();
    }

    public async Task<NotificationTemplateDto?> GetByIdAsync(int id)
    {
        return await _repository.GetDtoByIdAsync(id);
    }

    public async Task<NotificationTemplateDto> CreateAsync(CreateNotificationTemplateDto dto)
    {
        var template = _mapper.Map<NotificationTemplate>(dto);

        await _repository.AddAsync(template);
        await _repository.SaveChangesAsync();

        var resultDto = await _repository.GetDtoByIdAsync(template.IdNotificationTemplates.GetValueOrDefault());
        return resultDto ?? _mapper.Map<NotificationTemplateDto>(template);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, UpdateNotificationTemplateDto dto)
    {
        var template = await _repository.GetEntityByIdAsync(id);
        if (template == null)
            return (false, StatusCodes.Status404NotFound, "Template not found.");

        _mapper.Map(dto, template);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _repository.ExistsAsync(id))
                return (false, StatusCodes.Status404NotFound, "Template not found.");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }
}