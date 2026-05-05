using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Application.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;

    public GradeService(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public async Task<PaginatedResponseDto<GradeStudentDto>> GetMainDisciplineStudentsAsync(GradeQueryDto queryDto)
    {
        var (totalCount, items) = await _gradeRepository.GetMainDisciplineStudentsPagedAsync(queryDto);
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        return new PaginatedResponseDto<GradeStudentDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = queryDto
        };
    }

    public async Task<PaginatedResponseDto<GradeStudentDto>> GetSelectiveDisciplineStudentsAsync(GradeQueryDto queryDto)
    {
        var (totalCount, items) = await _gradeRepository.GetSelectiveDisciplineStudentsPagedAsync(queryDto);
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize);

        return new PaginatedResponseDto<GradeStudentDto>
        {
            TotalItems = totalCount,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = queryDto
        };
    }

    public async Task<bool> SetMainDisciplineGradeAsync(SetGradeDto dto)
    {
        var bind = await _gradeRepository.GetMainBindAsync(dto.IdBind);
        if (bind == null) return false;

        bind.Grade = dto.Grade;
        await _gradeRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetSelectiveDisciplineGradeAsync(SetGradeDto dto)
    {
        var bind = await _gradeRepository.GetSelectiveBindAsync(dto.IdBind);
        if (bind == null) return false;

        bind.Grade = dto.Grade;
        await _gradeRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<InstructorDisciplineDto>> GetMainDisciplinesByInstructorAsync(int adminId, int catalogYearId, bool isEvenSemester)
    {
        return await _gradeRepository.GetMainDisciplinesByInstructorAsync(adminId, catalogYearId, isEvenSemester);
    }

    public async Task<List<InstructorDisciplineDto>> GetSelectiveDisciplinesByInstructorAsync(int adminId, int catalogYearId, bool isEvenSemester)
    {
        return await _gradeRepository.GetSelectiveDisciplinesByInstructorAsync(adminId, catalogYearId, isEvenSemester);
    }

    public async Task<AcademicPeriodDto> GetAcademicPeriodAsync(DateTime date)
    {
        return await _gradeRepository.GetAcademicPeriodAsync(date);
    }
}
