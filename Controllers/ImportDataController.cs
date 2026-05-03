using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Text.Json;
using OlimpBack.Data;

namespace OlimpBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportDataController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ImportDataController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // STUDENT
        [HttpPost("student/update-or-create")]
        [HttpPost("student/update-or-create-from-file")]
        public async Task<IActionResult> UpdateOrCreateStudentFromFile([FromQuery] string fileName)
        {
            try
            {
                var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
                var fullPath = Path.Combine(parsedFilesPath, fileName);

                if (!System.IO.File.Exists(fullPath))
                    return NotFound(new { message = "Parsed file not found" });

                var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);

                var dtos = JsonSerializer.Deserialize<List<CreateStudentDto>>(jsonContent);

                if (dtos == null || dtos.Count == 0)
                    return BadRequest(new { message = "No data found in file" });

                var results = new List<StudentDto>();
                foreach (var dto in dtos)
                {
                    if (string.IsNullOrWhiteSpace(dto.NameStudent))
                        continue;
                    var student = await _context.Students
                        .FirstOrDefaultAsync(s => s.IdStudent == dto.IdStudent);
                    if (student == null)
                    {
                        // Create user for student if not exists
                        int userId = dto.UserId;
                        if (userId == 0)
                            userId = await UserService.CreateUserForStudent(dto.NameStudent, _context);
                        dto.UserId = userId;
                        student = _mapper.Map<Student>(dto);
                        _context.Students.Add(student);
                    }
                    else
                    {
                        // Only update non-null fields
                        if (dto.NameStudent != null) student.NameStudent = dto.NameStudent;
                        if (dto.StatusId != 0) student.EducationStatusId = dto.StatusId;
                        if (dto.EducationStart != default) student.EducationStart = dto.EducationStart;
                        if (dto.EducationEnd != default) student.EducationEnd = dto.EducationEnd;
                        if (dto.Course != 0) student.Course = dto.Course;
                        if (dto.FacultyId != 0) student.FacultyId = dto.FacultyId;
                        if (dto.EducationalDegreeId != 0) student.EducationalDegreeId = dto.EducationalDegreeId;
                        if (dto.StudyFormId != 0) student.StudyFormId = dto.StudyFormId;
                        if (dto.IsShort != 0) student.IsShort = dto.IsShort;
                        if (dto.EducationalProgramId != 0) student.EducationalProgramId = dto.EducationalProgramId;
                        if (dto.UserId != 0) student.UserId = dto.UserId;
                    }
                    await _context.SaveChangesAsync();
                    results.Add(_mapper.Map<StudentDto>(student));
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        // ADD DISCIPLINE WITH DETAILS
        [HttpPost("add-discipline-with-details/update-or-create")]
        public async Task<IActionResult> UpdateOrCreateSelectiveDisciplineWithDetails([FromQuery] string fileName)
        {
            var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
            var fullPath = Path.Combine(parsedFilesPath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "Parsed file not found" });

            var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);

            var dtos = JsonSerializer.Deserialize<List<CreateSelectiveDisciplineWithDetailsDto>>(jsonContent);

            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "No data found in file" });

            var results = new List<CreateSelectiveDisciplineWithDetailsDto>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.NameSelectiveDisciplines) || string.IsNullOrWhiteSpace(dto.CodeSelectiveDisciplines))
                    continue;
                var discipline = await _context.SelectiveDisciplines
                    .Include(d => d.SelectiveDetail)
                    .FirstOrDefaultAsync(d => d.CodeSelectiveDisciplines == dto.CodeSelectiveDisciplines);
                if (discipline == null)
                {
                    discipline = _mapper.Map<SelectiveDiscipline>(dto);
                    _context.SelectiveDisciplines.Add(discipline);
                    await _context.SaveChangesAsync();
                    var details = _mapper.Map<SelectiveDetail>(dto.Details);
                    details.IdSelectiveDetails = discipline.IdSelectiveDisciplines;
                    _context.SelectiveDetails.Add(details);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.NameSelectiveDisciplines)) discipline.NameSelectiveDisciplines = dto.NameSelectiveDisciplines;
                    if (!string.IsNullOrWhiteSpace(dto.CodeSelectiveDisciplines)) discipline.CodeSelectiveDisciplines = dto.CodeSelectiveDisciplines;
                    if (dto.FacultyId != 0) discipline.FacultyId = dto.FacultyId;
                    if (dto.MinCountPeople.HasValue) discipline.MinCountPeople = dto.MinCountPeople;
                    if (dto.MaxCountPeople.HasValue) discipline.MaxCountPeople = dto.MaxCountPeople;
                    if (dto.MinCourse.HasValue) discipline.MinCourse = dto.MinCourse;
                    if (dto.MaxCourse.HasValue) discipline.MaxCourse = dto.MaxCourse;
                    if (dto.IsEven.HasValue && dto.IsEven >= sbyte.MinValue && dto.IsEven <= sbyte.MaxValue)
                        discipline.IsEven = (sbyte)dto.IsEven.Value;
                    if (dto.DegreeLevelId.HasValue) discipline.DegreeLevelId = dto.DegreeLevelId;
                    // Details
                    if (discipline.SelectiveDetail == null)
                    {
                        discipline.SelectiveDetail = _mapper.Map<SelectiveDetail>(dto.Details);
                        discipline.SelectiveDetail.IdSelectiveDetails = discipline.IdSelectiveDisciplines;
                        _context.SelectiveDetails.Add(discipline.SelectiveDetail);
                    }
                    else if (dto.Details != null)
                    {
                        var det = dto.Details;
                        if (det.DepartmentId.HasValue) discipline.SelectiveDetail.DepartmentId = det.DepartmentId;
                        if (!string.IsNullOrWhiteSpace(det.Content.Teacher)) discipline.SelectiveDetail.Teachers = det.Content.Teacher;
                        if (!string.IsNullOrWhiteSpace(det.Content.Recomend)) discipline.SelectiveDetail.Recomend = det.Content.Recomend;
                        if (!string.IsNullOrWhiteSpace(det.Content.Prerequisites)) discipline.SelectiveDetail.Prerequisites = det.Content.Prerequisites;
                        if (!string.IsNullOrWhiteSpace(det.Content.Language)) discipline.SelectiveDetail.Language = det.Content.Language;
                        if (!string.IsNullOrWhiteSpace(det.Content.Provision )) discipline.SelectiveDetail.Provision = det.Content.Provision ;
                        if (!string.IsNullOrWhiteSpace(det.Content.WhyInterestingDetermination)) discipline.SelectiveDetail.WhyInterestingDetermination = det.Content.WhyInterestingDetermination;
                        if (!string.IsNullOrWhiteSpace(det.Content.ResultEducation)) discipline.SelectiveDetail.ResultEducation = det.Content.ResultEducation;
                        if (!string.IsNullOrWhiteSpace(det.Content.UsingIrl)) discipline.SelectiveDetail.UsingIrl = det.Content.UsingIrl;
                        if (!string.IsNullOrWhiteSpace(det.Content.DisciplineTopics)) discipline.SelectiveDetail.DisciplineTopics = det.Content.DisciplineTopics;
                        if (!string.IsNullOrWhiteSpace(det.Content.TypesOfTraining)) discipline.SelectiveDetail.TypesOfTraining = det.Content.TypesOfTraining;
                        if (!string.IsNullOrWhiteSpace(det.Content.TypeOfControll)) discipline.SelectiveDetail.TypeOfControll.Type= det.Content.TypeOfControll;
                    }
                }
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // EDUCATIONAL PROGRAM
        [HttpPost("educational-program/update-or-create")]
        public async Task<IActionResult> UpdateOrCreateEducationalProgram([FromQuery] string fileName)
        {
            var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
            var fullPath = Path.Combine(parsedFilesPath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "Parsed file not found" });

            var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);

            var dtos = JsonSerializer.Deserialize<List<CreateEducationalProgramDto>>(jsonContent);

            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "No data found in file" });

            var results = new List<EducationalProgramDto>();

            foreach (CreateEducationalProgramDto dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.NameEducationalProgram))
                    continue;
                var program = await _context.EducationalPrograms
                    .FirstOrDefaultAsync(p => p.NameEducationalProgram == dto.NameEducationalProgram);
                if (program == null)
                {
                    program = _mapper.Map<EducationalProgram>(dto);
                    _context.EducationalPrograms.Add(program);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.NameEducationalProgram)) program.NameEducationalProgram = dto.NameEducationalProgram;
                    if (dto.SelectiveDisciplineBySemestr != null && dto.SelectiveDisciplineBySemestr.Any())
                        program.SelectiveDisciplineBySemestr = dto.SelectiveDisciplineBySemestr;
                    program.DegreeId = dto.DegreeId;
                    if (dto.SpecialityId != 0) program.SpecialityId = dto.SpecialityId;
                    if (dto.Accreditation != 0) program.Accreditation = dto.Accreditation;
                    if (!string.IsNullOrWhiteSpace(dto.AccreditationType)) program.AccreditationType = dto.AccreditationType;
                }
                await _context.SaveChangesAsync();
                results.Add(_mapper.Map<EducationalProgramDto>(program));
            }
            return Ok(results);
        }

        // GROUP
        [HttpPost("group/update-or-create")]
        public async Task<IActionResult> UpdateOrCreateGroup([FromQuery] string fileName)
        {
            var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
            var fullPath = Path.Combine(parsedFilesPath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "Parsed file not found" });

            var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);

            var dtos = JsonSerializer.Deserialize<List<CreateGroupDto>>(jsonContent);

            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "No data found in file" });

            var results = new List<GroupDto>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.GroupCode))
                    continue;
                var group = await _context.StudentGroups
                    .FirstOrDefaultAsync(g => g.GroupCode == dto.GroupCode);
                if (group == null)
                {
                    group = _mapper.Map<StudentGroup>(dto);
                    _context.StudentGroups.Add(group);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.GroupCode)) group.GroupCode = dto.GroupCode;
                    if (dto.NumberOfStudents.HasValue) group.NumberOfStudents = dto.NumberOfStudents;
                }
                await _context.SaveChangesAsync();
                results.Add(_mapper.Map<GroupDto>(group));
            }
            return Ok(results);
        }

        // FACULTY
        [HttpPost("faculty/update-or-create")]
        public async Task<IActionResult> UpdateOrCreateFaculty([FromQuery] string fileName)
        {
            var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
            var fullPath = Path.Combine(parsedFilesPath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "Parsed file not found" });

            var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);

            var dtos = JsonSerializer.Deserialize<List<FacultyCreateDto>>(jsonContent);

            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "No data found in file" });

            var results = new List<FacultyDto>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.NameFaculty))
                    continue;
                var faculty = await _context.Faculties
                    .FirstOrDefaultAsync(f => f.NameFaculty == dto.NameFaculty);
                if (faculty == null)
                {
                    faculty = _mapper.Map<Faculty>(dto);
                    _context.Faculties.Add(faculty);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.NameFaculty)) faculty.NameFaculty = dto.NameFaculty;
                    if (!string.IsNullOrWhiteSpace(dto.Abbreviation)) faculty.Abbreviation = dto.Abbreviation;
                }
                await _context.SaveChangesAsync();
                results.Add(_mapper.Map<FacultyDto>(faculty));
            }
            return Ok(results);
        }

        // DEPARTMENT
        [HttpPost("department/update-or-create")]
        public async Task<IActionResult> UpdateOrCreateDepartment([FromQuery] string fileName)
        {
            var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
            var fullPath = Path.Combine(parsedFilesPath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "Parsed file not found" });

            var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);

            var dtos = JsonSerializer.Deserialize<List<CreateDepartmentDto>>(jsonContent);

            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "No data found in file" });

            var results = new List<DepartmentDto>();
            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.NameDepartment))
                    continue;
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.NameDepartment == dto.NameDepartment);
                if (department == null)
                {
                    department = _mapper.Map<Department>(dto);
                    _context.Departments.Add(department);
                }
                else
                {
                    if (dto.FacultyId != 0) department.FacultyId = dto.FacultyId;
                    if (!string.IsNullOrWhiteSpace(dto.NameDepartment)) department.NameDepartment = dto.NameDepartment;
                    if (!string.IsNullOrWhiteSpace(dto.Abbreviation)) department.Abbreviation = dto.Abbreviation;
                }
                await _context.SaveChangesAsync();
                results.Add(_mapper.Map<DepartmentDto>(department));
            }
            return Ok(results);
        }
    }
}