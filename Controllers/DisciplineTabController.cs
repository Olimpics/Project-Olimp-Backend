using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

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

        private async Task<int> CalculateCurrentCourse(Student student)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var yearsSinceAdmission = currentDate.Year - student.EducationStart.Year;
            
            // If we haven't reached July of the current academic year, we're still in the previous course
            if (currentDate.Month < 7)
            {
                yearsSinceAdmission--;
            }
            
            // Calculate the new course (students start from course 1)
            int calculatedCourse = yearsSinceAdmission + 1;

            // Update the student's course if it has changed
            if (student.Course != calculatedCourse)
            {
                student.Course = calculatedCourse;
                await _context.SaveChangesAsync();
            }

            return calculatedCourse;
        }

        private bool IsDisciplineAvailableForStudent(AddDiscipline discipline, Student student, int currentCourse, int countOfPeople)
        {
            // ��� ������� �� ����������
            if (student.BindAddDisciplines.Any(b => b.AddDisciplinesId == discipline.IdAddDisciplines))
                return false;

            // ������� ����������� �� ���������
            if (!string.IsNullOrEmpty(discipline.DegreeLevel) &&
                discipline.DegreeLevel != student.EducationalDegree.NameEducationalDegreec)
                return false;

            // ���� ������ ������������
            if (discipline.MinCourse.HasValue && (currentCourse + 1) < discipline.MinCourse)
                return false;

            // ���� ������ �������������
            if (discipline.MaxCourse.HasValue && (currentCourse + 1) > discipline.MaxCourse)
                return false;

            // �������� ��������
            if (discipline.AddSemestr.HasValue)
            {
                bool currentIsEven = DateTime.Now.Month switch
                {
                    >= 2 and <= 6 => true,
                    >= 9 and <= 12 => true,
                    _ => false
                };
            }

            // �������� ����� ���������
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

            int currentCourse = await CalculateCurrentCourse(student);

            var disciplines = await _context.AddDisciplines
                .Where(d => d.AddSemestr == (isEvenSemester ? (sbyte)0 : (sbyte)1))
                .Where(d => d.DegreeLevel == student.EducationalDegree.NameEducationalDegreec)
                .ToListAsync();

            var disciplineCounts = await _context.BindAddDisciplines
                .Where(b => b.InProcess == (sbyte)1)
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
                    IdAddDisciplines = d.IdAddDisciplines,
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
        [HttpPost("AddDisciplineBind")]
        public async Task<ActionResult> AddDisciplineBind(AddDisciplineBindDto dto)
        {
            try
            {
                // Check if student exists
                var student = await _context.Students
                    .Include(s => s.EducationalDegree)
                    .Include(s => s.BindAddDisciplines)
                    .FirstOrDefaultAsync(s => s.IdStudents == dto.StudentId);

                if (student == null)
                {
                    return NotFound(new { error = $"Student not found {dto.StudentId}" });
                }

                if (dto.Semester != 0 && dto.Semester != 1)
                {
                    return NotFound(new { error = "Semester non binary like 1 or 0 " });
                }

                int currentCourse = await CalculateCurrentCourse(student);

                // Calculate target semester (next course)
                int targetCourse = currentCourse + 1;
                int targetSemester = (targetCourse * 2) - dto.Semester;

                // Validate semester
                if (targetSemester > 8)
                {
                    return BadRequest(new { error = $"Invalid semester. Expected semester for next course is {targetSemester}" });
                }

                // Check if discipline exists
                var discipline = await _context.AddDisciplines
                    .FirstOrDefaultAsync(d => d.IdAddDisciplines == dto.DisciplineId);

                if (discipline == null)
                {
                    return NotFound(new { error = "Discipline not found" });
                }

                // Check if student is already enrolled in this discipline
                if (student.BindAddDisciplines.AsQueryable().Any(b => b.AddDisciplinesId == dto.DisciplineId))
                {
                    return BadRequest(new { error = "Student is already enrolled in this discipline" });
                }

                // Get count of people for the discipline
                var countOfPeople = await _context.BindAddDisciplines
                    .AsQueryable()
                    .Where(b => b.AddDisciplinesId == dto.DisciplineId && b.InProcess == (sbyte)1)
                    .CountAsync();

                // Check if discipline is available for the student
                if (!IsDisciplineAvailableForStudent(discipline, student, currentCourse, countOfPeople))
                {
                    return BadRequest(new { error = "Discipline is not available for this student" });
                }

                // Create new bind
                var bindDiscipline = new BindAddDiscipline
                {
                    StudentId = dto.StudentId,
                    AddDisciplinesId = dto.DisciplineId,
                    Semestr = targetSemester,
                    InProcess = (sbyte)1,
                    Loans = 5 // Default value
                };
                
                _context.BindAddDisciplines.Add(bindDiscipline);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Discipline successfully bound to student",
                    bindId = bindDiscipline.IdBindAddDisciplines
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request", details = ex.Message });
            }
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

            int currentCourse = await CalculateCurrentCourse(student);

            var allDisciplines = await _context.AddDisciplines.ToListAsync();
            var disciplineCounts = await _context.BindAddDisciplines
                .Where(b => b.InProcess == (sbyte)1)
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