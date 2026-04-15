using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IFilterRepository
{
    Task<(int TotalCount, List<SpecialityFilterDto> Items)> GetAddDisciplinesPagedAsync(AddDisciplineFilterQueryDto queryDto);
}

public class FilterRepository : IFilterRepository
{
    private readonly AppDbContext _context;

    public FilterRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalCount, List<SpecialityFilterDto> Items)> GetAddDisciplinesPagedAsync(AddDisciplineFilterQueryDto queryDto)
    {
        var query = _context.AddDisciplines
            .AsNoTracking()
            .AsQueryable();

        if (queryDto.CatalogYearId is > 0)
        {
            query = query.Where(ad => ad.IdCatalog == queryDto.CatalogYearId.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var searchLower = queryDto.Search.Trim().ToLower();
            query = query.Where(ad =>
                EF.Functions.Like(ad.CodeAddDisciplines.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(ad.NameAddDisciplines.ToLower(), $"%{searchLower}%"));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(ad => ad.NameAddDisciplines)
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(ad => new SpecialityFilterDto
            {
                Id = ad.IdAddDisciplines,
                Code = ad.CodeAddDisciplines,
                Name = ad.NameAddDisciplines
            })
            .ToListAsync();

        return (totalCount, items);
    }
}
