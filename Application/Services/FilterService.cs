using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Application.Services;

public interface IFilterService
{
    Task<PaginatedResponseDto<SpecialityFilterDto>> GetAddDisciplinesPagedAsync(AddDisciplineFilterQueryDto queryDto);
}

public class FilterService : IFilterService
{
    private readonly IFilterRepository _repository;

    public FilterService(IFilterRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResponseDto<SpecialityFilterDto>> GetAddDisciplinesPagedAsync(AddDisciplineFilterQueryDto queryDto)
    {
        var page = queryDto.Page <= 0 ? 1 : queryDto.Page;
        var pageSize = queryDto.PageSize <= 0 ? 20 : queryDto.PageSize;

        var normalized = new AddDisciplineFilterQueryDto
        {
            Page = page,
            PageSize = pageSize,
            Search = queryDto.Search,
            CatalogYearId = queryDto.CatalogYearId
        };

        var (totalCount, items) = await _repository.GetAddDisciplinesPagedAsync(normalized);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginatedResponseDto<SpecialityFilterDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = page,
            PageSize = pageSize,
            Items = items
        };
    }
}
