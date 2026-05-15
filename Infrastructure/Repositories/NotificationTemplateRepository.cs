using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;


namespace OlimpBack.Infrastructure.Database.Repositories;

public interface INotificationTemplateRepository
{
    Task<IEnumerable<NotificationTemplateDto>> GetAllDtoAsync();
    Task<NotificationTemplateDto?> GetDtoByIdAsync(Guid id);
    Task<NotificationTemplate?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(NotificationTemplate template);
    Task SaveChangesAsync();
}

public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public NotificationTemplateRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetAllDtoAsync()
    {
        return await _context.NotificationTemplates
            .AsNoTracking()
            .ProjectTo<NotificationTemplateDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<NotificationTemplateDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.NotificationTemplates
            .AsNoTracking()
            .Where(nt => nt.IdNotificationTemplates == id)
            .ProjectTo<NotificationTemplateDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<NotificationTemplate?> GetEntityByIdAsync(Guid id)
    {
        return await _context.NotificationTemplates.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.NotificationTemplates.AnyAsync(nt => nt.IdNotificationTemplates == id);
    }

    public async Task AddAsync(NotificationTemplate template)
    {
        await _context.NotificationTemplates.AddAsync(template);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}