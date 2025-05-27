using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;
using OlimpBack.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DisciplineTabController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetDisciplinesBySemester")]
        public async Task<ActionResult<DisciplineTabResponseDto>> GetDisciplinesBySemester(
            [FromQuery] int studentId,
            [FromQuery] bool isEvenSemester)
        {
            // Get the student with educational degree to determine their current course
            var student = await _context.Students
                .Include(s => s.EducationalDegree)
                .Include(s => s.BindAddDisciplines)
                .FirstOrDefaultAsync(s => s.IdStudents == studentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Calculate the current course based on education start date
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var yearsDifference = currentDate.Year - student.EducationStart.Year;
            var currentCourse = yearsDifference + 1;

            // Get all available disciplines that match the semester parity and degree level
            var disciplines = await _context.AddDisciplines
                .Where(d => d.AddSemestr == (isEvenSemester ? (sbyte)0 : (sbyte)1)) // Check semester type (0 for even, 1 for odd)
                .Where(d => d.DegreeLevel == student.EducationalDegree.NameEducationalDegreec) // Check degree level
                .ToListAsync();

            // Get count of people for each discipline
            var disciplineCounts = await _context.BindAddDisciplines
                .Where(b => b.InProcess == true)
                .GroupBy(b => b.AddDisciplinesId)
                .Select(g => new { DisciplineId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DisciplineId, x => x.Count);

            // Filter out disciplines that student is already enrolled in
            var availableDisciplines = disciplines
                .Where(d => !student.BindAddDisciplines.Any(b => b.AddDisciplinesId == d.IdAddDisciplines))
                .Select(d => {
                    var countOfPeople = disciplineCounts.ContainsKey(d.IdAddDisciplines) ? disciplineCounts[d.IdAddDisciplines] : 0;
                    var isAvailable = !d.MaxCountPeople.HasValue || countOfPeople < d.MaxCountPeople.Value;

                    return new FullDisciplineDto
                    {
                        IdAddDisciplines = d.IdAddDisciplines,
                        NameAddDisciplines = d.NameAddDisciplines,
                        CodeAddDisciplines = d.CodeAddDisciplines,
                        Faculty = d.Faculty,
                        Department = d.Department,
                        MinCountPeople = d.MinCountPeople,
                        MaxCountPeople = d.MaxCountPeople,
                        MinCourse = d.MinCourse,
                        MaxCourse = d.MaxCourse,
                        AddSemestr = d.AddSemestr,
                        Recomend = d.Recomend,
                        Teacher = d.Teacher,
                        Prerequisites = d.Prerequisites,
                        DegreeLevel = d.DegreeLevel,
                        IsAvailable = isAvailable,
                        CountOfPeople = countOfPeople
                    };
                })
                .ToList();

            // Map the result to DTO
            var response = new DisciplineTabResponseDto
            {
                StudentId = student.IdStudents,
                StudentName = student.NameStudent,
                CurrentCourse = currentCourse,
                IsEvenSemester = isEvenSemester,
                Disciplines = availableDisciplines
            };

            return Ok(response);
        }

        [HttpGet("GetAllDisciplinesWithAvailability")]
        public async Task<ActionResult<List<DisciplineWithAvailabilityDto>>> GetAllDisciplinesWithAvailability(
            [FromQuery] int studentId)
        {
            // Get the student with educational degree to determine their current course
            var student = await _context.Students
                .Include(s => s.EducationalDegree)
                .Include(s => s.BindAddDisciplines)
                .FirstOrDefaultAsync(s => s.IdStudents == studentId);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Calculate the current course based on education start date
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var yearsDifference = currentDate.Year - student.EducationStart.Year;
            var currentCourse = yearsDifference + 1;

            // Get all disciplines
            var allDisciplines = await _context.AddDisciplines.ToListAsync();
            var result = new List<DisciplineWithAvailabilityDto>();

            foreach (var discipline in allDisciplines)
            {
                var disciplineDto = _mapper.Map<DisciplineWithAvailabilityDto>(discipline);
                disciplineDto.IsAvailable = true;

                // Check if discipline is already taken by the student
                if (student.BindAddDisciplines.Any(b => b.AddDisciplinesId == discipline.IdAddDisciplines))
                {
                    disciplineDto.IsAvailable = false;
                    result.Add(disciplineDto);
                    continue;
                }

                // Check degree level
                if (discipline.DegreeLevel != null && discipline.DegreeLevel != student.EducationalDegree.NameEducationalDegreec)
                {
                    disciplineDto.IsAvailable = false;
                    result.Add(disciplineDto);
                    continue;
                }

                // Check course restrictions
                if (discipline.MinCourse != null && currentCourse < discipline.MinCourse)
                {
                    disciplineDto.IsAvailable = false;
                    result.Add(disciplineDto);
                    continue;
                }

                if (discipline.MaxCourse != null && currentCourse > discipline.MaxCourse)
                {
                    disciplineDto.IsAvailable = false;
                    result.Add(disciplineDto);
                    continue;
                }

                // Check semester restrictions
                if (discipline.AddSemestr != null)
                {
                    var isEvenSemester = currentDate.Month >= 9 && currentDate.Month <= 12 || currentDate.Month >= 2 && currentDate.Month <= 6;
                    var disciplineSemester = discipline.AddSemestr == 0;
                    
                    if (isEvenSemester != disciplineSemester)
                    {
                        disciplineDto.IsAvailable = false;
                        result.Add(disciplineDto);
                        continue;
                    }
                }

                result.Add(disciplineDto);
            }

            return Ok(result);
        }
    }
} 