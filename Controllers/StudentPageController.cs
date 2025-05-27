using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Controllers;
[Route("api/[controller]")]
[ApiController]
public class StudentPageController:ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentPageController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<AddDisciplineDto>>> GetAddDisciplines(int id)
    {
        var disciplines = await _context.AddDisciplines
            .ProjectTo<AddDisciplineDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(disciplines);
    }

    [HttpGet("disciplines/{studentId}")]
    public async Task<ActionResult<StudentDisciplinesDto>> GetStudentDisciplines(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.BindAddDisciplines)
                .ThenInclude(bad => bad.AddDisciplines)
            .Include(s => s.EducationalProgram)
                .ThenInclude(ep => ep.BindMainDisciplines)
            .FirstOrDefaultAsync(s => s.IdStudents == studentId);

        if (student == null)
        {
            return NotFound();
        }

        var result = new StudentDisciplinesDto
        {
            StudentId = student.IdStudents,
            StudentName = student.NameStudent,
            MainDisciplines = _mapper.Map<List<BindMainDisciplineDto>>(student.EducationalProgram.BindMainDisciplines),
            AdditionalDisciplines = _mapper.Map<List<BindAddDisciplineDto>>(student.BindAddDisciplines)
        };

        return Ok(result);
    }

    [HttpGet("disciplines/by-semester/{studentId}")]
    public async Task<ActionResult<StudentDisciplinesBySemesterDTO>> GetStudentDisciplinesBySemester(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.BindAddDisciplines)
                .ThenInclude(bad => bad.AddDisciplines)
            .Include(s => s.EducationalProgram)
                .ThenInclude(ep => ep.BindMainDisciplines)
            .Include(s => s.EducationalDegree)
            .FirstOrDefaultAsync(s => s.IdStudents == studentId);

        if (student == null)
        {
            return NotFound();
        }

        var result = new StudentDisciplinesBySemesterDTO
        {
            StudentId = student.IdStudents,
            StudentName = student.NameStudent,
            DegreeName = student.EducationalDegree.NameEducationalDegreec
        };

        // Determine number of semesters based on degree
        int maxSemesters = student.EducationalDegree.NameEducationalDegreec == "Бакалавр" ? 8 : 2;

        // Initialize dictionaries for the appropriate number of semesters
        for (int i = 1; i <= maxSemesters; i++)
        {
            result.MainDisciplinesBySemester[i] = new List<BindMainDisciplineDto>();
            result.AdditionalDisciplinesBySemester[i] = new List<BindAddDisciplineDto>();
        }

        // Group main disciplines by semester
        var mainDisciplines = _mapper.Map<List<BindMainDisciplineDto>>(student.EducationalProgram.BindMainDisciplines);
        foreach (var discipline in mainDisciplines)
        {
            if (discipline.Semestr >= 1 && discipline.Semestr <= maxSemesters)
            {
                result.MainDisciplinesBySemester[discipline.Semestr].Add(discipline);
            }
        }

        // Group additional disciplines by semester
        var additionalDisciplines = _mapper.Map<List<BindAddDisciplineDto>>(student.BindAddDisciplines);
        foreach (var discipline in additionalDisciplines)
        {
            if (discipline.Semestr >= 1 && discipline.Semestr <= maxSemesters)
            {
                result.AdditionalDisciplinesBySemester[discipline.Semestr].Add(discipline);
            }
        }

        return Ok(result);
    }
}