using System;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IImportService
{
    Task<string> ImportSelectiveDisciplinesAsync(SelectiveDisciplineImportRequestDto request);
    Task<(byte[] content, string fileName)> GetSelectiveDisciplineFileAsync(string fileName);
}
