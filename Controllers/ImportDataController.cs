using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;
using OlimpBack.Utils;
using System.Text.Json;

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
        public async Task<IActionResult> UpdateOrCreateAddDisciplineWithDetails([FromQuery] string fileName)
        {
            var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
            var fullPath = Path.Combine(parsedFilesPath, fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "Parsed file not found" });

            var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);

            var dtos = JsonSerializer.Deserialize<List<CreateAddDisciplineWithDetailsDto>>(jsonContent);

            if (dtos == null || dtos.Count == 0)
                return BadRequest(new { message = "No data found in file" });

            var results = new List<CreateAddDisciplineWithDetailsDto>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.NameAddDisciplines) || string.IsNullOrWhiteSpace(dto.CodeAddDisciplines))
                    continue;
                var discipline = await _context.AddDisciplines
                    .Include(d => d.AddDetail)
                    .FirstOrDefaultAsync(d => d.CodeAddDisciplines == dto.CodeAddDisciplines);
                if (discipline == null)
                {
                    discipline = _mapper.Map<AddDiscipline>(dto);
                    _context.AddDisciplines.Add(discipline);
                    await _context.SaveChangesAsync();
                    var details = _mapper.Map<AddDetail>(dto.Details);
                    details.IdAddDetails = discipline.IdAddDisciplines;
                    _context.AddDetails.Add(details);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.NameAddDisciplines)) discipline.NameAddDisciplines = dto.NameAddDisciplines;
                    if (!string.IsNullOrWhiteSpace(dto.CodeAddDisciplines)) discipline.CodeAddDisciplines = dto.CodeAddDisciplines;
                    if (dto.FacultyId != 0) discipline.FacultyId = dto.FacultyId;
                    if (dto.MinCountPeople.HasValue) discipline.MinCountPeople = dto.MinCountPeople;
                    if (dto.MaxCountPeople.HasValue) discipline.MaxCountPeople = dto.MaxCountPeople;
                    if (dto.MinCourse.HasValue) discipline.MinCourse = dto.MinCourse;
                    if (dto.MaxCourse.HasValue) discipline.MaxCourse = dto.MaxCourse;
                    if (dto.IsEven.HasValue && dto.IsEven >= sbyte.MinValue && dto.IsEven <= sbyte.MaxValue)
                        discipline.IsEven = (sbyte)dto.IsEven.Value;
                    if (dto.DegreeLevelId.HasValue) discipline.DegreeLevelId = dto.DegreeLevelId;
                    // Details
                    if (discipline.AddDetail == null)
                    {
                        discipline.AddDetail = _mapper.Map<AddDetail>(dto.Details);
                        discipline.AddDetail.IdAddDetails = discipline.IdAddDisciplines;
                        _context.AddDetails.Add(discipline.AddDetail);
                    }
                    else if (dto.Details != null)
                    {
                        var det = dto.Details;
                        if (det.DepartmentId.HasValue) discipline.AddDetail.DepartmentId = det.DepartmentId;
                        if (!string.IsNullOrWhiteSpace(det.Content.Teacher)) discipline.AddDetail.Teachers = det.Content.Teacher;
                        if (!string.IsNullOrWhiteSpace(det.Content.Recomend)) discipline.AddDetail.Recomend = det.Content.Recomend;
                        if (!string.IsNullOrWhiteSpace(det.Content.Prerequisites)) discipline.AddDetail.Prerequisites = det.Content.Prerequisites;
                        if (!string.IsNullOrWhiteSpace(det.Content.Language)) discipline.AddDetail.Language = det.Content.Language;
                        if (!string.IsNullOrWhiteSpace(det.Content.Provision )) discipline.AddDetail.Provision = det.Content.Provision ;
                        if (!string.IsNullOrWhiteSpace(det.Content.WhyInterestingDetermination)) discipline.AddDetail.WhyInterestingDetermination = det.Content.WhyInterestingDetermination;
                        if (!string.IsNullOrWhiteSpace(det.Content.ResultEducation)) discipline.AddDetail.ResultEducation = det.Content.ResultEducation;
                        if (!string.IsNullOrWhiteSpace(det.Content.UsingIrl)) discipline.AddDetail.UsingIrl = det.Content.UsingIrl;
                        if (!string.IsNullOrWhiteSpace(det.Content.DisciplineTopics)) discipline.AddDetail.DisciplineTopics = det.Content.DisciplineTopics;
                        if (!string.IsNullOrWhiteSpace(det.Content.TypesOfTraining)) discipline.AddDetail.TypesOfTraining = det.Content.TypesOfTraining;
                        if (!string.IsNullOrWhiteSpace(det.Content.TypeOfControll)) discipline.AddDetail.TypeOfControll = det.Content.TypeOfControll;
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
                    if (dto.CountAddSemestr3.HasValue) program.CountAddSemestr3 = dto.CountAddSemestr3;
                    if (dto.CountAddSemestr4.HasValue) program.CountAddSemestr4 = dto.CountAddSemestr4;
                    if (dto.CountAddSemestr5.HasValue) program.CountAddSemestr5 = dto.CountAddSemestr5;
                    if (dto.CountAddSemestr6.HasValue) program.CountAddSemestr6 = dto.CountAddSemestr6;
                    if (dto.CountAddSemestr7.HasValue) program.CountAddSemestr7 = dto.CountAddSemestr7;
                    if (dto.CountAddSemestr8.HasValue) program.CountAddSemestr8 = dto.CountAddSemestr8;
                    program.DegreeId = dto.DegreeId;
                    if (!string.IsNullOrWhiteSpace(dto.Speciality)) program.Speciality = dto.Speciality;
                    if (dto.Accreditation != 0) program.Accreditation = dto.Accreditation;
                    if (!string.IsNullOrWhiteSpace(dto.AccreditationType)) program.AccreditationType = dto.AccreditationType;
                    if (dto.StudentsAmount != 0) program.StudentsAmount = dto.StudentsAmount;
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
                var group = await _context.Groups
                    .FirstOrDefaultAsync(g => g.GroupCode == dto.GroupCode);
                if (group == null)
                {
                    group = _mapper.Map<Group>(dto);
                    _context.Groups.Add(group);
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