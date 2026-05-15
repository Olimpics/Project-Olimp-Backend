using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IBindLoansMainService
{
    Task<PaginatedResponseDto<BindLoansMainDto>> GetBindLoansMainAsync(BindLoansMainQueryDto queryDto);

    Task<BindLoansMainDto?> GetBindLoansMainAsync(Guid id);
    Task<BindLoansMainDto> CreateBindLoansMainAsync(CreateBindLoansMainDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateBindLoansMainAsync(Guid id, UpdateBindLoansMainDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteBindLoansMainAsync(Guid id);
}

