using System;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IImportService
{
    Task<string> ImportSelectiveDisciplinesAsync(SelectiveDisciplineImportRequestDto request);
    Task<string> ImportGroupsAsync(IFormFile file);
    Task<string> ImportStudentsAsync(IFormFile file);
    Task<string> CreateStudentUsersAsync(IFormFile file);
    Task<(byte[] content, string fileName)> GetSelectiveDisciplineFileAsync(string fileName);
}
