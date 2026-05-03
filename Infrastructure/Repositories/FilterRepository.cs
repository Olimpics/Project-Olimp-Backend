using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Data;

namespace OlimpBack.Infrastructure.Database.Repositories;

public interface IFilterRepository
{
    Task<(int TotalCount, List<SpecialityFilterDto> Items)> GetSelectiveDisciplinesPagedAsync(SelectiveDisciplineFilterQueryDto queryDto);
}

public class FilterRepository : IFilterRepository
{
    private readonly AppDbContext _context;

    public FilterRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int TotalCount, List<SpecialityFilterDto> Items)> GetSelectiveDisciplinesPagedAsync(SelectiveDisciplineFilterQueryDto queryDto)
    {
        var query = _context.SelectiveDisciplines
            .AsNoTracking()
            .AsQueryable();

        if (queryDto.CatalogYearId is > 0)
        {
            query = query.Where(ad => ad.CatalogId == queryDto.CatalogYearId.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var searchLower = queryDto.Search.Trim().ToLower();
            query = query.Where(ad =>
                EF.Functions.Like(ad.CodeSelectiveDisciplines.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(ad.NameSelectiveDisciplines.ToLower(), $"%{searchLower}%"));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(ad => ad.NameSelectiveDisciplines)
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(ad => new SpecialityFilterDto
            {
                Id = ad.IdSelectiveDisciplines,
                Code = ad.CodeSelectiveDisciplines ?? "",
                Name = ad.NameSelectiveDisciplines ?? ""
            })
            .ToListAsync();

        return (totalCount, items);
    }
}
