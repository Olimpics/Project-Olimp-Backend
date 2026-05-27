using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public interface IExcelProcessingService
{
    Task<List<GroupExcelRowDto>> ExtractGroupsAsync(IFormFile file);
    Task<List<StudentExcelRowDto>> ExtractStudentsAsync(IFormFile file);
    Task<List<DepartmentExcelRowDto>> ExtractDepartmentsAsync(IFormFile file);
    Task<List<BranchExcelRowDto>> ExtractBranchesAsync(IFormFile file);
    Task<List<SpecialityExcelRowDto>> ExtractSpecialitiesAsync(IFormFile file);
}

public class GroupExcelRowDto
{
    public string? StartOfStudy { get; set; }
    public string? FormOfStudy { get; set; }
    public string? EducationDegree { get; set; }
    public string? TermOfStudy { get; set; } // Tak/Ni
    public string? EducationalProgram { get; set; }
    public string? Course { get; set; }
    public string? GroupCode { get; set; }
}

public class StudentExcelRowDto
{
    public string? EdboCode { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? ThirdName { get; set; }
    public string? EducationStart { get; set; }
    public string? EducationEnd { get; set; }
    public string? GroupCode { get; set; }
    public string? EducationStatus { get; set; }
    public string? IsFunded { get; set; } // Budget/Contract
    public string? Email { get; set; }
    public string? ReportCard { get; set; }
}

public class DepartmentExcelRowDto
{
    public string? FacultyName { get; set; }
    public string? DepartmentName { get; set; }
    public string? Abbreviation { get; set; }
}

public class BranchExcelRowDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
}

public class SpecialityExcelRowDto
{
    public string? BranchRaw { get; set; } // Column 1
    public string? SpecialityRaw { get; set; } // Column 2
    public string? SpecializationRaw { get; set; } // Column 3
}
