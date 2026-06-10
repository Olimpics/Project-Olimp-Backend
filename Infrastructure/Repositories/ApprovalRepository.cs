using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Models;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IApprovalRepository
{
    Task<IEnumerable<ApprovalDto>> GetAllDtoAsync();
    Task<ApprovalDto?> GetDtoByIdAsync(Guid id);
    Task<Approval?> GetEntityByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(Approval entity);
    Task<int> DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public class ApprovalRepository : IApprovalRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ApprovalRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ApprovalDto>> GetAllDtoAsync()
    {
        return await _context.Approvals
            .AsNoTracking()
            .ProjectTo<ApprovalDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ApprovalDto?> GetDtoByIdAsync(Guid id)
    {
        return await _context.Approvals
            .AsNoTracking()
            .Where(a => a.IdApproval == id)
            .ProjectTo<ApprovalDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<Approval?> GetEntityByIdAsync(Guid id)
    {
        return await _context.Approvals.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Approvals.AnyAsync(a => a.IdApproval == id);
    }

    public async Task AddAsync(Approval entity)
    {
        await _context.Approvals.AddAsync(entity);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        return await _context.Approvals
            .Where(a => a.IdApproval == id)
            .ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
