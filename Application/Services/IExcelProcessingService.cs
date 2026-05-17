using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public interface IExcelProcessingService
{
    Task<List<GroupExcelRowDto>> ExtractGroupsAsync(IFormFile file);
    Task<List<StudentExcelRowDto>> ExtractStudentsAsync(IFormFile file);
}

public class GroupExcelRowDto
{
    public string? StartOfStudy { get; set; }
    public string? FormOfStudy { get; set; }
    public string? TermOfStudy { get; set; } // Tak/Ni
    public string? EducationalProgram { get; set; }
    public string? Course { get; set; }
    public string? GroupCode { get; set; }
}

public class StudentExcelRowDto
{
    public string? EdboCode { get; set; }
    public string? NameStudent { get; set; }
    public string? EducationStart { get; set; }
    public string? EducationEnd { get; set; }
    public string? GroupCode { get; set; }
    public string? EducationStatus { get; set; }
    public string? IsFunded { get; set; } // Budget/Contract
    public string? Email { get; set; }
    public string? ReportCard { get; set; }
}
