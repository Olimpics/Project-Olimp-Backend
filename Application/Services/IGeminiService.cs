using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IGeminiService
{
    Task<List<GeminiSelectiveDisciplineDto>> ProcessSelectiveDisciplinesAsync(List<SelectiveDisciplineWordContentDto> batch);
}
