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

        private bool IsDisciplineAvailableForStudent(AddDiscipline discipline, Student student, int currentCourse, int countOfPeople)
        {
            // Уже записан на дисциплину
            if (student.BindAddDisciplines.Any(b => b.AddDisciplinesId == discipline.IdAddDisciplines))
                return false;

            // Уровень образования не совпадает
            if (!string.IsNullOrEmpty(discipline.DegreeLevel) &&
                discipline.DegreeLevel != student.EducationalDegree.NameEducationalDegreec)
                return false;

            // Курс меньше минимального
            if (discipline.MinCourse.HasValue && currentCourse < discipline.MinCourse)
                return false;

            // Курс больше максимального
            if (discipline.MaxCourse.HasValue && currentCourse > discipline.MaxCourse)
                return false;

            // Проверка семестра
            if (discipline.AddSemestr.HasValue)
            {
                bool currentIsEven = DateTime.Now.Month switch
                {
                    >= 2 and <= 6 => true,
                    >= 9 and <= 12 => true,
                    _ => false
                };

                bool disciplineIsEven = discipline.AddSemestr == 0;
                if (currentIsEven != disciplineIsEven)
                    return false;
            }

            // Превышен лимит студентов
            if (discipline.MaxCountPeople.HasValue && countOfPeople >= discipline.MaxCountPeople.Value)
                return false;

            return true;
        }
        [HttpGet("GetDisciplinesBySemester")]
        public async Task<ActionResult<DisciplineTabResponseDto>> GetDisciplinesBySemester(
            [FromQuery] int studentId,
            [FromQuery] bool isEvenSemester)
        {
            var student = await _context.Students
                .Include(s => s.EducationalDegree)
                .Include(s => s.BindAddDisciplines)
                .FirstOrDefaultAsync(s => s.IdStudents == studentId);

            if (student == null)
                return NotFound("Student not found");

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            int currentCourse = currentDate.Year - student.EducationStart.Year + 1;

            var disciplines = await _context.AddDisciplines
                .Where(d => d.AddSemestr == (isEvenSemester ? (sbyte)0 : (sbyte)1))
                .Where(d => d.DegreeLevel == student.EducationalDegree.NameEducationalDegreec)
                .ToListAsync();

            var disciplineCounts = await _context.BindAddDisciplines
                .Where(b => b.InProcess)
                .GroupBy(b => b.AddDisciplinesId)
                .Select(g => new { DisciplineId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DisciplineId, x => x.Count);

            var availableDisciplines = disciplines
                .Where(d =>
                {
                    int count = disciplineCounts.TryGetValue(d.IdAddDisciplines, out var c) ? c : 0;
                    return IsDisciplineAvailableForStudent(d, student, currentCourse, count);
                })
                .Select(d => new SimpleDisciplineDto
                {
                    NameAddDisciplines = d.NameAddDisciplines,
                    CodeAddDisciplines = d.CodeAddDisciplines
                })
                .ToList();

            return Ok(new DisciplineTabResponseDto
            {
                StudentId = student.IdStudents,
                StudentName = student.NameStudent,
                CurrentCourse = currentCourse,
                IsEvenSemester = isEvenSemester,
                Disciplines = availableDisciplines
            });
        }
        [HttpGet("GetAllDisciplinesWithAvailability")]
        public async Task<ActionResult<List<FullDisciplineDto>>> GetAllDisciplinesWithAvailability([FromQuery] int studentId)
        {
            var student = await _context.Students
                .Include(s => s.EducationalDegree)
                .Include(s => s.BindAddDisciplines)
                .FirstOrDefaultAsync(s => s.IdStudents == studentId);

            if (student == null)
                return NotFound("Student not found");

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            int currentCourse = currentDate.Year - student.EducationStart.Year + 1;

            var allDisciplines = await _context.AddDisciplines.ToListAsync();
            var disciplineCounts = await _context.BindAddDisciplines
                .Where(b => b.InProcess)
                .GroupBy(b => b.AddDisciplinesId)
                .Select(g => new { DisciplineId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DisciplineId, x => x.Count);

            var result = new List<FullDisciplineDto>();

            foreach (var discipline in allDisciplines)
            {
                var dto = _mapper.Map<FullDisciplineDto>(discipline);
                int count = disciplineCounts.TryGetValue(discipline.IdAddDisciplines, out var c) ? c : 0;
                dto.CountOfPeople = count;

                dto.IsAvailable = IsDisciplineAvailableForStudent(discipline, student, currentCourse, count);
                result.Add(dto);
            }

            return Ok(result);
        }

    }
} 