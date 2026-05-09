using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IWordProcessingService
{
    Task<SelectiveDisciplineWordContentDto> ExtractContentAsync(string filePath);
}
