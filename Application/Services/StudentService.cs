using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;
using OlimpBack.Utils;

namespace OlimpBack.Application.Services;

public class StudentService : IStudentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<StudentForCatalogDto>> GetStudentsAsync(StudentQueryDto queryDto)
    {
        var query = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(s => EF.Functions.Like(s.NameStudent.ToLower(), $"%{lowerSearch}%"));
        }

        if (queryDto.Faculties != null && queryDto.Faculties.Any())
        {
            query = query.Where(s => s.Group.EducationalProgram.Speciality.Department.FacultyId != null && queryDto.Faculties.Contains(s.Group.EducationalProgram.Speciality.Department.FacultyId));
        }

        if (queryDto.Specialities != null && queryDto.Specialities.Any())
        {
            query = query.Where(s => s.Group.EducationalProgram.SpecialityId != Guid.Empty && queryDto.Specialities.Contains(s.Group.EducationalProgram.SpecialityId));
        }

        if (queryDto.GroupIds != null && queryDto.GroupIds.Any())
            query = query.Where(s => s.GroupId != Guid.Empty && queryDto.GroupIds.Contains(s.GroupId));

        if (queryDto.Courses != null && queryDto.Courses.Any())
            query = query.Where(s => queryDto.Courses.Contains(s.Group.Course));

        if (queryDto.StudyFormIds != null && queryDto.StudyFormIds.Any())
            query = query.Where(s => s.Group.StudyFormId != Guid.Empty && queryDto.StudyFormIds.Contains(s.Group.StudyFormId));

        if (queryDto.DegreeLevelIds != null && queryDto.DegreeLevelIds.Any())
            query = query.Where(s => s.Group.EducationalProgram.DegreeId != Guid.Empty && queryDto.DegreeLevelIds.Contains(s.Group.EducationalProgram.DegreeId));

        if (queryDto.IsShort.HasValue)
        {
            query = query.Where(s => s.Group.IsAccelerated == queryDto.IsShort.Value);
        }

        query = queryDto.SortOrder switch
        {
            1 => query.OrderByDescending(d => d.NameStudent),
            2 => query.OrderBy(d => d.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation),
            3 => query.OrderByDescending(d => d.Group.EducationalProgram.Speciality.Department.Faculty.Abbreviation),
            4 => query.OrderBy(d => d.Group.GroupCode),
            5 => query.OrderByDescending(d => d.Group.GroupCode),
            _ => query.OrderBy(d => d.NameStudent)
        };

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        var items = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ProjectTo<StudentForCatalogDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PaginatedResponseDto<StudentForCatalogDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = queryDto
        };
    }

    public async Task<StudentDto?> GetStudentAsync(Guid id)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.IdStudent == id)
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<StudentDto>> CreateStudentsAsync(IReadOnlyList<CreateStudentDto> dtos)
    {
        var results = new List<StudentDto>();
        var studentsToAdd = new List<Student>();

        var namesToCheck = dtos.Where(d => !string.IsNullOrWhiteSpace(d.NameStudent)).Select(d => d.NameStudent).ToList();
        var existingStudents = await _context.Students
            .Where(s => namesToCheck.Contains(s.NameStudent))
            .Select(s => new { s.IdStudent, s.NameStudent })
            .ToListAsync();

        foreach (var dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.NameStudent))
                continue;

            if (existingStudents.Any(s => s.NameStudent == dto.NameStudent && s.IdStudent == dto.IdStudent))
                continue;

            var userId = dto.UserId;
            if (userId == Guid.Empty)
            {
                userId = await UserService.CreateUserForStudent(dto.NameStudent, _context);
            }
            dto.UserId = userId;

            var student = _mapper.Map<Student>(dto);
            studentsToAdd.Add(student);
        }

        if (studentsToAdd.Any())
        {
            _context.Students.AddRange(studentsToAdd);
            await _context.SaveChangesAsync();

            results.AddRange(_mapper.Map<List<StudentDto>>(studentsToAdd));
        }

        return results;
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> UpdateStudentAsync(Guid id, UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return (false, StatusCodes.Status404NotFound, "Student not found");

        _mapper.Map(dto, student);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Students.AnyAsync(s => s.IdStudent == id);
            if (!exists)
                return (false, StatusCodes.Status404NotFound, "Student not found");

            throw;
        }

        return (true, StatusCodes.Status204NoContent, null);
    }
}
