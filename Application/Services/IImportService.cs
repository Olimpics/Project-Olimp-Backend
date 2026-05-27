using System;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IImportService
{
    Task<string> ImportSelectiveDisciplinesAsync(SelectiveDisciplineImportRequestDto request);
    Task<string> ImportEducationalProgramAsync(EducationalProgramImportRequestDto request);
    Task<string> ImportEducationalProgramBatchAsync(EducationalProgramBatchImportRequestDto request);
    Task<string> ImportGroupsAsync(IFormFile file);
    Task<string> ImportStudentsAsync(IFormFile file);
    Task<string> CreateStudentUsersAsync(IFormFile file);
    Task<string> ImportDepartmentsAsync(IFormFile file);
    Task<(byte[] content, string fileName)> GetSelectiveDisciplineFileAsync(string fileName);
}
