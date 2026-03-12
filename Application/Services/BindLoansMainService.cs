using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class BindLoansMainService : IBindLoansMainService
{
    private readonly AppDbContext _context;

    public BindLoansMainService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponseDto<BindLoansMainDto>> GetBindLoansMainAsync(BindLoansMainQueryDto queryDto)
    {
        var query = _context.BindLoansMains
            .Include(b => b.AddDisciplines)
            .Include(b => b.EducationalProgram)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(b =>
                EF.Functions.Like(b.AddDisciplines.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(b.AddDisciplines.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(b.EducationalProgram.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(b.EducationalProgram.SpecialityCode.ToLower(), $"%{lowerSearch}%"));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.AddDisciplinesIds))
        {
            var disciplineIdList = queryDto.AddDisciplinesIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (disciplineIdList.Any())
            {
                query = query.Where(b => disciplineIdList.Contains(b.AddDisciplinesId));
            }
        }

        if (!string.IsNullOrWhiteSpace(queryDto.EducationalProgramIds))
        {
            var programIdList = queryDto.EducationalProgramIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var val) ? val : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (programIdList.Any())
            {
                query = query.Where(b => programIdList.Contains(b.EducationalProgramId));
            }
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        var bindings = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        bindings = queryDto.SortOrder switch
        {
            1 => bindings.OrderByDescending(d => d.AddDisciplines.CodeAddDisciplines).ToList(),
            2 => bindings.OrderBy(d => d.EducationalProgram.SpecialityCode).ToList(),
            3 => bindings.OrderByDescending(d => d.EducationalProgram.SpecialityCode).ToList(),
            _ => bindings.OrderBy(d => d.AddDisciplines.CodeAddDisciplines).ToList()
        };

        var items = bindings.Select(b => new BindLoansMainDto
        {
            IdBindLoan = b.IdBindLoan,
            AddDisciplinesId = b.AddDisciplinesId,
            EducationalProgramId = b.EducationalProgramId,
            CodeAddDisciplines = b.AddDisciplines.CodeAddDisciplines,
            AddDisciplineName = b.AddDisciplines.NameAddDisciplines,
            SpecialityCode = b.EducationalProgram.SpecialityCode,
            EducationalProgramName = b.EducationalProgram.NameEducationalProgram
        }).ToList();

        return new PaginatedResponseDto<BindLoansMainDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items
        };
    }

    public async Task<BindLoansMainDto?> GetBindLoansMainAsync(int id)
    {
        var binding = await _context.BindLoansMains
            .Include(b => b.AddDisciplines)
            .Include(b => b.EducationalProgram)
            .FirstOrDefaultAsync(b => b.IdBindLoan == id);

        if (binding == null)
            return null;

        return new BindLoansMainDto
        {
            IdBindLoan = binding.IdBindLoan,
            AddDisciplinesId = binding.AddDisciplinesId,
            EducationalProgramId = binding.EducationalProgramId,
            CodeAddDisciplines = binding.AddDisciplines.CodeAddDisciplines,
            AddDisciplineName = binding.AddDisciplines.NameAddDisciplines,
            SpecialityCode = binding.EducationalProgram.SpecialityCode,
            EducationalProgramName = binding.EducationalProgram.NameEducationalProgram
        };
    }

    public async Task<BindLoansMainDto> CreateBindLoansMainAsync(CreateBindLoansMainDto dto)
    {
        var binding = new BindLoansMain
        {
            AddDisciplinesId = dto.AddDisciplinesId,
            EducationalProgramId = dto.EducationalProgramId
        };

        _context.BindLoansMains.Add(binding);
        await _context.SaveChangesAsync();

        binding = await _context.BindLoansMains
            .Include(b => b.AddDisciplines)
            .Include(b => b.EducationalProgram)
            .FirstAsync(b => b.IdBindLoan == binding.IdBindLoan);

        return new BindLoansMainDto
        {
            IdBindLoan = binding.IdBindLoan,
            AddDisciplinesId = binding.AddDisciplinesId,
            EducationalProgramId = binding.EducationalProgramId,
            CodeAddDisciplines = binding.AddDisciplines.CodeAddDisciplines,
            AddDisciplineName = binding.AddDisciplines.NameAddDisciplines,
            SpecialityCode = binding.EducationalProgram.SpecialityCode,
            EducationalProgramName = binding.EducationalProgram.NameEducationalProgram
        };
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateBindLoansMainAsync(int id, UpdateBindLoansMainDto dto)
    {
        if (id != dto.IdBindLoan)
            return (false, StatusCodes.Status400BadRequest, "Route id does not match body id.");

        var binding = await _context.BindLoansMains.FindAsync(id);
        if (binding == null)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        binding.AddDisciplinesId = dto.AddDisciplinesId;
        binding.EducationalProgramId = dto.EducationalProgramId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.BindLoansMains.AnyAsync(b => b.IdBindLoan == id);
            if (!exists)
                return (false, StatusCodes.Status404NotFound, "Binding not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> DeleteBindLoansMainAsync(int id)
    {
        var binding = await _context.BindLoansMains.FindAsync(id);
        if (binding == null)
            return (false, StatusCodes.Status404NotFound, "Binding not found");

        _context.BindLoansMains.Remove(binding);
        await _context.SaveChangesAsync();

        return (true, StatusCodes.Status204NoContent, null);
    }
}

