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

    public DisciplineTabService(IDisciplineTabRepository repository, IMapper mapper, AppDbContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<PaginatedResponseDto<FullDisciplineDto>?> GetAllDisciplinesWithAvailabilityAsync(GetAllDisciplinesWithAvailabilityQueryDto queryDto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(queryDto.StudentId, _context);
        if (context == null) return null;

        var allDisciplines = await _repository.GetDisciplinesForAvailabilityAsync(queryDto);

        var fullList = allDisciplines.Select(discipline =>
        {
            var dto = _mapper.Map<FullDisciplineDto>(discipline);
            if (context.DisciplineCounts.TryGetValue(discipline.IdSelectiveDisciplines, out var c))
            {
                dto.CountOfPeople = c;
            }
            else
            {
                dto.CountOfPeople = 0;
            }
            dto.IsAvailable = DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context);
            return dto;
        });

        if (queryDto.OnlyAvailable)
            fullList = fullList.Where(d => d.IsAvailable);

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

        var availableDisciplines = disciplines
            .Where(d => DisciplineAvailabilityService.IsDisciplineAvailable(d, context))
            .Select(d => new SimpleDisciplineDto
            {
                IdSelectiveDisciplines = d.IdSelectiveDisciplines,
                NameSelectiveDisciplines = d.NameSelectiveDisciplines ?? "",
                CodeSelectiveDisciplines = d.CodeSelectiveDisciplines ?? ""
            })
            .ToList();

        return new DisciplineTabResponseDto
        {
            StudentId = context.Student.IdStudent,
            StudentName = context.Student.NameStudent ?? "",
            CurrentCourse = context.CurrentCourse,
            IsEvenSemester = queryDto.IsEvenSemester,
            Disciplines = availableDisciplines
        };
    }

    public async Task<(Guid? bindId, string? error)> SelectiveDisciplineBindAsync(SelectiveDisciplineBindDto dto)
    {
        var context = await DisciplineAvailabilityService.BuildAvailabilityContext(dto.StudentId, _context);
        if (context == null) return (null, $"Student not found {dto.StudentId}");
        if (dto.Semestr != 0 && dto.Semestr != 1) return (null, "Semestr must be 0 or 1");

        int targetCourse = context.CurrentCourse + 1;
        int targetSemester = targetCourse * 2 - dto.Semestr;

        if (targetCourse > 4) return (null, "You can't choose disciplines in 5th course");
        if (targetSemester > 8) return (null, $"Invalid semester: {targetSemester}");
        if (context.BoundDisciplineIds.Contains(dto.DisciplineId)) return (null, "Student is already enrolled");

        var discipline = await _repository.GetDisciplineByIdAsNoTrackingAsync(dto.DisciplineId);
        if (discipline == null) return (null, "Discipline not found");
        if (!DisciplineAvailabilityService.IsDisciplineAvailable(discipline, context))
            return (null, "Discipline is not available for this student");

        var bind = new BindSelectiveDiscipline
        {
            StudentId = dto.StudentId,
            SelectiveDisciplineId = dto.DisciplineId,
            Semestr = targetSemester,
            InProcess = true,
            Loans = dto.Loans
        };

        await _repository.AddBindAsync(bind);
        await _repository.SaveChangesAsync();

        return (bind.IdBindSelectiveDisciplines, null);
    }

    public async Task<FullDisciplineWithDetailsDto?> GetDisciplineWithDetailsAsync(Guid id) =>
        await _repository.GetDisciplineWithDetailsDtoAsync(id);

    public async Task<FullDisciplineWithDetailsDto?> CreateDisciplineWithDetailsAsync(CreateSelectiveDisciplineWithDetailsDto dto)
    {
        var discipline = _mapper.Map<SelectiveDiscipline>(dto, opts => opts.Items["DbContext"] = _context);
        var details = _mapper.Map<SelectiveDetail>(dto.Details);

        discipline.IdSelectiveDisciplines = Guid.NewGuid();
        details.IdSelectiveDetails = discipline.IdSelectiveDisciplines;
        discipline.SelectiveDetail = details;
        
        // Initial status
        var initialStatus = await _context.Approvals.OrderBy(a => a.ApprobalLevel).FirstOrDefaultAsync();
        if (initialStatus != null)
        {
            discipline.ApprovalStatusId = initialStatus.IdApproval;
        }

        await _repository.SelectiveDisciplineAsync(discipline);
        await _repository.SaveChangesAsync();

        // Sync teachers and recommendations after we have the ID
        await SyncTeachersAndBindingsAsync(discipline, dto.AdminIds, dto.Details.Content.Teacher);
        await SyncRecommendedJsonAndEpAsync(discipline, dto.RecomendationBranches, dto.RecomendationSpeciality, dto.RecomendationEducationalProgram);
        
        await _repository.SaveChangesAsync();

        return await _repository.GetDisciplineWithDetailsDtoAsync(discipline.IdSelectiveDisciplines);
    }

    public async Task<(bool success, string? error)> UpdateDisciplineWithDetailsAsync(Guid id, UpdateSelectiveDisciplineWithDetailsDto dto)
    {
        var discipline = await _repository.GetDisciplineWithDetailEntityAsync(id);
        if (discipline == null) return (false, "Discipline not found");

        if (dto.Details.DepartmentId.HasValue)
        {
            if (!await _repository.DepartmentExistsAsync(dto.Details.DepartmentId.Value))
                return (false, $"Department with ID {dto.Details.DepartmentId.Value} does not exist");
        }

        if (discipline.SelectiveDetail == null)
        {
            discipline.SelectiveDetail = new SelectiveDetail { IdSelectiveDetails = discipline.IdSelectiveDisciplines };
        }

        _mapper.Map(dto, discipline, opts => opts.Items["DbContext"] = _context);
        
        // Manual mapping for topics to handle indices
        if (dto.Details.Content.ChangedTopicIndices != null && dto.Details.Content.ChangedTopicIndices.Any() && dto.Details.Content.DisciplineTopics != null)
        {
            var currentTopics = discipline.SelectiveDetail.DisciplineTopics ?? new List<string>();
            foreach (var index in dto.Details.Content.ChangedTopicIndices)
            {
                if (index >= 0 && index < dto.Details.Content.DisciplineTopics.Count)
                {
                    var newTopic = dto.Details.Content.DisciplineTopics[index];
                    if (index < currentTopics.Count)
                        currentTopics[index] = newTopic;
                    else
                        currentTopics.Add(newTopic);
                }
            }
            discipline.SelectiveDetail.DisciplineTopics = currentTopics;
        }
        else if (dto.Details.Content.DisciplineTopics != null)
        {
            discipline.SelectiveDetail.DisciplineTopics = dto.Details.Content.DisciplineTopics;
        }

        // Map other details fields except topics which we handled
        var topicsTemp = discipline.SelectiveDetail.DisciplineTopics;
        _mapper.Map(dto.Details.Content, discipline.SelectiveDetail);
        discipline.SelectiveDetail.DisciplineTopics = topicsTemp;

        await SyncTeachersAndBindingsAsync(discipline, dto.AdminIds, dto.Details.Content.Teacher);
        await SyncRecommendedJsonAndEpAsync(discipline, dto.RecomendationBranches, dto.RecomendationSpeciality, dto.RecomendationEducationalProgram);

        await _repository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string? error)> UpdateDisciplineApprovalStatusAsync(Guid id, UpdateApprovalStatusDto dto)
    {
        var discipline = await _context.SelectiveDisciplines
            .Include(d => d.ApprovalStatus)
            .FirstOrDefaultAsync(d => d.IdSelectiveDisciplines == id);
            
        if (discipline == null) return (false, "Discipline not found");

        var currentLevel = discipline.ApprovalStatus?.ApprobalLevel ?? 0;
        
        Approval? nextStatus = null;
        if (dto.IsIncrease)
        {
            nextStatus = await _context.Approvals
                .Where(a => a.ApprobalLevel > currentLevel)
                .OrderBy(a => a.ApprobalLevel)
                .FirstOrDefaultAsync();
        }
        else
        {
            nextStatus = await _context.Approvals
                .Where(a => a.ApprobalLevel < currentLevel)
                .OrderByDescending(a => a.ApprobalLevel)
                .FirstOrDefaultAsync();
        }

        if (nextStatus != null)
        {
            discipline.ApprovalStatusId = nextStatus.IdApproval;
            await SendApprovalNotificationAsync(discipline, nextStatus, dto.Message);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        return (true, "No higher/lower approval level found, status unchanged.");
    }

    private async Task SendApprovalNotificationAsync(SelectiveDiscipline discipline, Approval status, string? customMessage)
    {
        var query = _context.AdminsPersonals.AsQueryable();

        if (status.ApprobalLevel == 1)
        {
            query = query.Where(a => a.DepartmentId == discipline.DepartmentId);
        }
        else if (status.ApprobalLevel == 2)
        {
            var dept = await _context.Departments.FindAsync(discipline.DepartmentId);
            if (dept != null)
            {
                query = query.Where(a => a.FacultyId == dept.FacultyId);
            }
        }

        var adminUserIds = await query.Select(a => a.UserId).ToListAsync();
        
        var targetUserIds = await _context.UserRoles
            .Where(ur => ur.RoleId == status.RoleId && adminUserIds.Contains(ur.UserId))
            .Select(ur => ur.UserId)
            .ToListAsync();

        if (status.ApprobalLevel == 3)
        {
            targetUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == status.RoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();
        }

        foreach (var userId in targetUserIds.Distinct())
        {
            var notification = new Notification
            {
                UserId = userId,
                CustomMessage = customMessage ?? $"Discipline \"{discipline.NameSelectiveDisciplines}\" status changed to {status.AppovalStatus}",
                IsRead = false,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            _context.Notifications.Add(notification);
        }
    }

    public async Task<(bool success, string? error)> UpdateDisciplineStatusAsync(Guid id, Guid statusId)
    {
        var discipline = await _repository.GetDisciplineWithDetailEntityAsync(id);
        if (discipline == null) return (false, "Discipline not found");

        discipline.ApprovalStatusId = statusId;
        await _repository.SaveChangesAsync();
        return (true, null);
    }

    private async Task SyncTeachersAndBindingsAsync(SelectiveDiscipline discipline, List<Guid>? adminIds, string? teachersText)
    {
        // Remove old bindings
        var oldBindings = _context.BindTeachersSelectives.Where(b => b.SelectiveDisciplinesId == discipline.IdSelectiveDisciplines);
        _context.BindTeachersSelectives.RemoveRange(oldBindings);

        if (adminIds != null && adminIds.Any())
        {
            // Add new bindings
            var newBindings = adminIds.Select((id, index) => new BindTeachersSelective
            {
                IdBindTeacherSelective = Guid.NewGuid(),
                AdminId = id,
                SelectiveDisciplinesId = discipline.IdSelectiveDisciplines,
                IsHead = index == 0 // First element is Head
            });
            await _context.BindTeachersSelectives.AddRangeAsync(newBindings);
        }

        if (discipline.SelectiveDetail == null)
        {
            discipline.SelectiveDetail = new SelectiveDetail { IdSelectiveDetails = discipline.IdSelectiveDisciplines };
        }
        // User said: "a text field will also be passed... entered into the Teachers field in the SelectiveDetail table"
        discipline.SelectiveDetail.Teachers = teachersText;
    }

    private async Task SyncRecommendedJsonAndEpAsync(SelectiveDiscipline discipline, List<Guid>? branchIds, List<Guid>? specialtyIds, List<Guid>? epIds)
    {
        var recommendedEpIds = new HashSet<Guid>();
        var recommendedJson = new Dictionary<string, object>();

        if (branchIds != null && branchIds.Any())
        {
            var epsFromBranches = await _context.EducationalPrograms
                .Where(ep => ep.Speciality.BranchId != Guid.Empty && branchIds.Contains(ep.Speciality.BranchId))
                .Select(ep => ep.IdEducationalProgram)
                .ToListAsync();
            foreach (var id in epsFromBranches) recommendedEpIds.Add(id);
            recommendedJson["Branches"] = branchIds;
        }

        if (specialtyIds != null && specialtyIds.Any())
        {
            var epsFromSpecs = await _context.EducationalPrograms
                .Where(ep => specialtyIds.Contains(ep.SpecialityId))
                .Select(ep => ep.IdEducationalProgram)
                .ToListAsync();
            foreach (var id in epsFromSpecs) recommendedEpIds.Add(id);
            recommendedJson["Specialties"] = specialtyIds;
        }

        if (epIds != null && epIds.Any())
        {
            foreach (var id in epIds) recommendedEpIds.Add(id);
            
            // Find similar groups for the specified EPs
            var groupsForSpecifiedEps = await _context.BindSimilaEducationalProgramInGroups
                .Where(b => epIds.Contains(b.EducationalProgramId))
                .Select(b => b.GroupId)
                .Distinct()
                .ToListAsync();

            if (groupsForSpecifiedEps.Any())
            {
                var similarEpIds = await _context.BindSimilaEducationalProgramInGroups
                    .Where(b => groupsForSpecifiedEps.Contains(b.GroupId))
                    .Select(b => b.EducationalProgramId)
                    .ToListAsync();
                foreach (var id in similarEpIds) recommendedEpIds.Add(id);
            }
            
            recommendedJson["EducationalPrograms"] = epIds;
        }

        discipline.RecommendedEp = recommendedEpIds.ToList();
        if (discipline.SelectiveDetail == null)
        {
            discipline.SelectiveDetail = new SelectiveDetail { IdSelectiveDetails = discipline.IdSelectiveDisciplines };
        }
        discipline.SelectiveDetail.Recommended = System.Text.Json.JsonSerializer.Serialize(recommendedJson);
    }
}
