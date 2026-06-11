using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;
using OlimpBack.Utils;
using OlimpBack.Data;

namespace OlimpBack.Application.Services;

public class DisciplineTabService : IDisciplineTabService
{
    private readonly IDisciplineTabRepository _repository;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly IStudentChoiceCacheService _cacheService;
    private readonly IDisciplineCacheService _disciplineCacheService;

    public DisciplineTabService(
        IDisciplineTabRepository repository, 
        IMapper mapper, 
        AppDbContext context, 
        IStudentChoiceCacheService cacheService,
        IDisciplineCacheService disciplineCacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
        _cacheService = cacheService;
        _disciplineCacheService = disciplineCacheService;
    }

    public async Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null) return null;

        var allDisciplines = await _repository.GetDisciplinesForAvailabilityAsync(queryDto);

        var fullList = new List<FullDisciplineDto>();
        foreach (var discipline in allDisciplines)
        {
            var dto = _mapper.Map<FullDisciplineDto>(discipline);
            
            // Use cache for occupancy
            dto.CountOfPeople = await _disciplineCacheService.GetOccupancyAsync(discipline.IdSelectiveDisciplines);
            
            dto.IsAvailable = DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context, dto.CountOfPeople);
            fullList.Add(dto);
        }

        if (queryDto.OnlyAvailable)
            fullList = fullList.Where(d => d.IsAvailable).ToList();

        var sortedList = queryDto.SortOrder switch
        {
            1 => fullList.OrderByDescending(d => d.NameSelectiveDisciplines).ToList(),
            2 => fullList.OrderBy(d => d.CountOfPeople).ToList(),
            3 => fullList.OrderByDescending(d => d.CountOfPeople).ToList(),
            4 => fullList.OrderBy(d => d.FacultyAbbreviation).ToList(),
            _ => fullList.OrderBy(d => d.NameSelectiveDisciplines).ToList()
        };

        var totalItems = sortedList.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);
        var paginatedResult = sortedList
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToList();

        return new PaginatedResponseDto<FullDisciplineDto>
        {
            TotalPages = totalPages,
            TotalItems = totalItems,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = paginatedResult
        };
    }
    public async Task<DisciplineTabResponseDto?> GetDisciplinesBySemesterAsync(GetDisciplinesBySemesterQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null) return null;

        var now = DateTime.UtcNow;

        if (context.Student.Group?.EducationalProgram?.Speciality?.Department?.FacultyId == null)
            return null;

        var isPeriodActive = await _repository.IsChoicePeriodActiveAsync(context.Student.Group.EducationalProgram.Speciality.Department.FacultyId, now);
        if (!isPeriodActive) return null;

        var disciplines = await _repository.GetDisciplinesBySemesterAsync(queryDto);

        var availableDisciplines = new List<SimpleDisciplineDto>();
        foreach (var d in disciplines)
        {
            int occupancy = await _disciplineCacheService.GetOccupancyAsync(d.IdSelectiveDisciplines);
            if (DisciplineAvailabilityService.IsDisciplineAvailable(d, context, occupancy))
            {
                availableDisciplines.Add(new SimpleDisciplineDto
                {
                    IdSelectiveDisciplines = d.IdSelectiveDisciplines,
                    NameSelectiveDisciplines = d.NameSelectiveDisciplines ?? "",
                    CodeSelectiveDisciplines = d.CodeSelectiveDisciplines ?? ""
                });
            }
        }

        return new DisciplineTabResponseDto
        {
            StudentId = context.Student.IdStudent,
            FirstName = context.Student.FirstName,
            SecondName = context.Student.SecondName ?? "",
            ThirdName = context.Student.ThirdName ?? "",
            CurrentCourse = context.CurrentCourse,
            IsEvenSemester = queryDto.IsEvenSemester,
            Disciplines = availableDisciplines
        };
    }

    public async Task<(Guid? bindId, string? error)> SelectiveDisciplineBindAsync(SelectiveDisciplineBindDto dto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(dto.StudentId, _context);
        if (context == null) return (null, $"Student not found {dto.StudentId}");

        // Check for open DisciplineChoicePeriod
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var studentDeptId = context.Student.Group.EducationalProgram.Speciality.DepartmentId;
        var studentDegreeId = context.Student.Group.EducationalProgram.DegreeId;
        var studentIsShort = context.Student.Group.EducationalProgram.IsAccelerated;
        var studentSpecialityId = context.Student.Group.EducationalProgram.SpecialityId;

        var activePeriods = await _context.DisciplineChoicePeriods
            .Include(p => p.CatalogYear)
            .Where(p => !p.IsClose && 
                        p.StartDate <= now && 
                        p.EndDate >= now &&
                        p.DepartmentId == studentDeptId &&
                        p.DegreeLevelId == studentDegreeId &&
                        p.IsShort == studentIsShort)
            .ToListAsync();

        var period = activePeriods.FirstOrDefault(p => p.SpecialityId == studentSpecialityId) 
                     ?? activePeriods.FirstOrDefault(p => p.SpecialityId == null);

        if (period == null)
            return (null, "No active choice period found for your program and department.");

        if (dto.Semestr != 0 && dto.Semestr != 1) return (null, "Semestr must be 0 or 1");

        bool isSpring = dto.Semestr == 0; 
        var limits = await _cacheService.GetLimitsAsync(dto.StudentId);
        int currentLimit = isSpring ? limits[1] : limits[0];

        if (currentLimit <= 0)
        {
            return (null, $"you have already selected all disciplines of the {(isSpring ? "spring" : "fall")} semester");
        }

        int targetCourse = period.CatalogYear.YearStart - context.Student.Group.AdmissionYear.Value.Year + 1;
        int targetSemester = targetCourse * 2 - dto.Semestr;

        if (targetCourse > 4) return (null, "You can't choose disciplines in 5th course");
        if (targetSemester > 8) return (null, $"Invalid semester: {targetSemester}");
        if (context.BoundDisciplineIds.Contains(dto.DisciplineId)) return (null, "Student is already enrolled");

        var discipline = await _context.SelectiveDisciplines
            .Include(d => d.Type)
            .FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == dto.DisciplineId);

        if (discipline == null) return (null, "Discipline not found");

        // Use cache for occupancy check
        int currentOccupancy = await _disciplineCacheService.GetOccupancyAsync(dto.DisciplineId);

        if (discipline.MaxCountPeople.HasValue && currentOccupancy >= discipline.MaxCountPeople.Value)
        {
            // Update status to "Enrolled"
            var enrolledStatus = await _context.TypeOfDisciplines.FirstOrDefaultAsync(t => t.TypeName == "Enrolled");
            if (enrolledStatus != null)
            {
                discipline.TypeId = enrolledStatus.IdTypeOfDiscipline;
                await _context.SaveChangesAsync();
            }
            return (null, "The maximum number of people has already been reached for this discipline.");
        }

        if (!DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context, currentOccupancy))
            return (null, "Discipline is not available for this student");

        var bind = new BindSelectiveDiscipline
        {
            StudentId = dto.StudentId,
            SelectiveDisciplineId = dto.DisciplineId,
            Semestr = targetSemester,
            InProcess = true,
            Loans = dto.Loans,
            YearId = period.CatalogYearId
        };

        await _repository.AddBindAsync(bind);
        await _repository.SaveChangesAsync();

        // Update caches
        await _cacheService.UpdateLimitAsync(dto.StudentId, isSpring, -1);
        await _disciplineCacheService.IncrementOccupancyAsync(dto.DisciplineId);

        return (bind.IdBindSelectiveDisciplines, null);
    }

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(Guid id) =>
        await _repository.GetDisciplineWithDetailsDtoAsync(id);

    public async Task<List<SimilarDisciplineGroupDto>> GetSimilarDisciplinesAsync(Guid disciplineId) =>
        await _repository.GetSimilarDisciplinesAsync(disciplineId);

    public async Task<bool> RemoveDisciplineFromSimilarityGroupAsync(Guid disciplineId, Guid groupId) =>
        await _repository.RemoveDisciplineFromSimilarityGroupAsync(disciplineId, groupId);
}
