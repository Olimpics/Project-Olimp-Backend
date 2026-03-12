using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IBindLoansMainService
{
    Task<PaginatedResponseDto<BindLoansMainDto>> GetBindLoansMainAsync(BindLoansMainQueryDto queryDto);

    Task<BindLoansMainDto?> GetBindLoansMainAsync(int id);

    Task<BindLoansMainDto> CreateBindLoansMainAsync(CreateBindLoansMainDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> UpdateBindLoansMainAsync(int id, UpdateBindLoansMainDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> DeleteBindLoansMainAsync(int id);
}

