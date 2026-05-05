using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Services;

public interface IGradeService
{
    Task<PaginatedResponseDto<GradeStudentDto>> GetMainDisciplineStudentsAsync(GradeQueryDto queryDto);
    Task<PaginatedResponseDto<GradeStudentDto>> GetSelectiveDisciplineStudentsAsync(GradeQueryDto queryDto);
    Task<bool> SetMainDisciplineGradeAsync(SetGradeDto dto);
    Task<bool> SetSelectiveDisciplineGradeAsync(SetGradeDto dto);
    Task<List<InstructorDisciplineDto>> GetMainDisciplinesByInstructorAsync(int adminId, int catalogYearId, bool isEvenSemester);
    Task<List<InstructorDisciplineDto>> GetSelectiveDisciplinesByInstructorAsync(int adminId, int catalogYearId, bool isEvenSemester);
    Task<AcademicPeriodDto> GetAcademicPeriodAsync(DateTime date);
}
