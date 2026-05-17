using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OlimpBack.Utils;

namespace OlimpBack.Application.Services;

public class ImportService : IImportService
{
    private readonly IWordProcessingService _wordService;
    private readonly IGeminiService _geminiService;
    private readonly IExcelProcessingService _excelService;
    private readonly IEmailService _emailService;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImportService> _logger;

    private readonly string _uploadPath;
    private readonly string _importedPath;
    private readonly string _errorsPath;

    public ImportService(
        IWordProcessingService wordService,
        IGeminiService geminiService,
        IExcelProcessingService excelService,
        IEmailService emailService,
        AppDbContext context,
        IWebHostEnvironment environment,
        ILogger<ImportService> logger)
    {
        _wordService = wordService;
        _geminiService = geminiService;
        _excelService = excelService;
        _emailService = emailService;
        _context = context;
        _environment = environment;
        _logger = logger;

        var baseDir = Path.Combine(_environment.ContentRootPath, "Uploads", "Selective");
        _uploadPath = Path.Combine(baseDir, "Pending");
        _importedPath = Path.Combine(baseDir, "Imported");
        _errorsPath = Path.Combine(baseDir, "Errors");

        Directory.CreateDirectory(_uploadPath);
        Directory.CreateDirectory(_importedPath);
        Directory.CreateDirectory(_errorsPath);
    }

    public async Task<string> ImportSelectiveDisciplinesAsync(SelectiveDisciplineImportRequestDto request)
    {
        var tempUnzipPath = Path.Combine(_uploadPath, Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempUnzipPath);

        try
        {
            // 1. Unzip
            using (var stream = request.Archive.OpenReadStream())
            using (var archive = new ZipArchive(stream))
            {
                archive.ExtractToDirectory(tempUnzipPath);
            }

            var files = Directory.GetFiles(tempUnzipPath, "*.docx", SearchOption.AllDirectories);
            var batches = files.Chunk(20);

            int totalProcessed = 0;
            int totalErrors = 0;

            foreach (var batch in batches)
            {
                var wordContents = new List<(string filePath, SelectiveDisciplineWordContentDto content)>();
                
                foreach (var filePath in batch)
                {
                    var content = await _wordService.ExtractContentAsync(filePath);
                    wordContents.Add((filePath, content));
                }

                try
                {
                    var geminiResults = await _geminiService.ProcessSelectiveDisciplinesAsync(wordContents.Select(x => x.content).ToList());

                    for (int i = 0; i < geminiResults.Count; i++)
                    {
                        var geminiDto = geminiResults[i];
                        var originalFile = wordContents[i].filePath;
                        
                        try
                        {
                            await SaveToDatabaseAsync(geminiDto, request.CatalogId, request.IsFaculty, originalFile);
                            totalProcessed++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error saving discipline {geminiDto.NameSelectiveDisciplines}");
                            MoveToError(originalFile, ex.Message);
                            totalErrors++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing batch with Gemini");
                    foreach (var item in wordContents)
                    {
                        MoveToError(item.filePath, "Gemini Batch Error: " + ex.Message);
                        totalErrors++;
                    }
                }
            }

            return $"Import finished. Success: {totalProcessed}, Errors: {totalErrors}";
        }
        finally
        {
            if (Directory.Exists(tempUnzipPath))
            {
                Directory.Delete(tempUnzipPath, true);
            }
        }
    }

    public async Task<string> ImportStudentsAsync(IFormFile file)
    {
        var excelRows = await _excelService.ExtractStudentsAsync(file);

        int successCount = 0;
        int errorCount = 0;

        foreach (var row in excelRows)
        {
            try
            {
                // 1. Lookup Group
                var group = await _context.StudentGroups
                    .Include(g => g.EducationalProgram)
                    .FirstOrDefaultAsync(g => g.GroupCode == row.GroupCode);
                if (group == null) throw new Exception($"Group '{row.GroupCode}' not found");

                // 2. Lookup EducationStatus
                var status = await _context.EducationStatuses
                    .FirstOrDefaultAsync(s => s.NameEducationStatus.ToLower() == row.EducationStatus.ToLower());
                if (status == null) throw new Exception($"Status '{row.EducationStatus}' not found");

                // 3. Dates
                if (!DateOnly.TryParse(row.EducationStart, out var start))
                    throw new Exception($"Invalid EducationStart: {row.EducationStart}");
                if (!DateOnly.TryParse(row.EducationEnd, out var end))
                    throw new Exception($"Invalid EducationEnd: {row.EducationEnd}");

                // 4. IsFunded
                bool isFunded = row.IsFunded?.Trim().Equals("Бюджет", StringComparison.OrdinalIgnoreCase) ?? true;

                // 5. Check for existing student (by EdboCode)
                var existingStudent = await _context.Students
                    .FirstOrDefaultAsync(s => s.EdboCode == row.EdboCode);

                Student student;
                if (existingStudent != null)
                {
                    student = existingStudent;
                    student.NameStudent = row.NameStudent ?? student.NameStudent;
                    student.EducationStart = start;
                    student.EducationEnd = end;
                    student.GroupId = group.IdGroup;
                    student.EducationStatusId = status.IdEducationStatus;
                    student.IsFunded = isFunded;
                    student.ReportCard = row.ReportCard ?? student.ReportCard;
                    student.Avail = true;
                }
                else
                {
                    student = new Student
                    {
                        IdStudent = Guid.NewGuid(),
                        EdboCode = row.EdboCode,
                        NameStudent = row.NameStudent ?? "Unknown",
                        EducationStart = start,
                        EducationEnd = end,
                        GroupId = group.IdGroup,
                        EducationStatusId = status.IdEducationStatus,
                        IsFunded = isFunded,
                        ReportCard = row.ReportCard ?? "Unknown",
                        Avail = true,
                        UserId = Guid.Empty // Placeholder
                    };
                    _context.Students.Add(student);
                }

                // Save to get the ID if new
                await _context.SaveChangesAsync();

                // 6. Link disciplines
                var mainDisciplines = await _context.MainDisciplines
                    .Where(md => md.EducationalProgramId == group.EducationalProgramId)
                    .ToListAsync();

                // Find CatalogYear
                var catalogYear = await _context.CatalogYears
                    .FirstOrDefaultAsync(cy => group.AdmissionYear.HasValue && cy.YearStart == group.AdmissionYear.Value.Year);

                if (catalogYear != null)
                {
                    foreach (var md in mainDisciplines)
                    {
                        var exists = await _context.BindMainDisciplines
                            .AnyAsync(bmd => bmd.StudentId == student.IdStudent && bmd.MainDisciplinesId == md.IdMainDisciplines);

                        if (!exists)
                        {
                            _context.BindMainDisciplines.Add(new BindMainDiscipline
                            {
                                IdBindMainDisciplines = Guid.NewGuid(),
                                StudentId = student.IdStudent,
                                MainDisciplinesId = md.IdMainDisciplines,
                                YearId = catalogYear.IdCatalog,
                                Semestr = md.Semestr,
                                Grade = 0,
                                IsRedo = false
                            });
                        }
                    }
                }

                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing student {row.EdboCode}");
                errorCount++;
            }
        }

        await _context.SaveChangesAsync();
        return $"Student import finished. Success: {successCount}, Errors: {errorCount}";
    }

    public async Task<string> ImportGroupsAsync(IFormFile file)
    {
        var excelRows = await _excelService.ExtractGroupsAsync(file);

        // 1. Set Avail = false for all groups
        await _context.StudentGroups.ExecuteUpdateAsync(s => s.SetProperty(g => g.Avail, false));

        int successCount = 0;
        int errorCount = 0;

        foreach (var row in excelRows)
        {
            try
            {
                // Parse year
                int year = 0;
                if (DateTime.TryParse(row.StartOfStudy, out var date))
                {
                    year = date.Year;
                }
                else if (int.TryParse(row.StartOfStudy, out var parsedYear))
                {
                    year = parsedYear;
                }

                if (year == 0) throw new Exception("Invalid Start of Study");

                // Lookup StudyForm
                var studyForm = await _context.StudyForms
                    .FirstOrDefaultAsync(sf => sf.NameStudyForm.ToLower() == row.FormOfStudy.ToLower());
                if (studyForm == null) throw new Exception($"Study form '{row.FormOfStudy}' not found");

                // Lookup EducationalProgram
                var ep = await _context.EducationalPrograms
                    .Include(e => e.Catalog)
                    .FirstOrDefaultAsync(e => e.NameEducationalProgram.ToLower() == row.EducationalProgram.ToLower() && e.Catalog.YearStart == year);
                if (ep == null) throw new Exception($"Educational program '{row.EducationalProgram}' for year {year} not found");

                // IsAccelerated
                bool isAccelerated = row.TermOfStudy?.Trim().Equals("Tak", StringComparison.OrdinalIgnoreCase) ?? false;


                // Course
                int course = int.TryParse(row.Course, out var c) ? c : 0;

                // AdmissionYear as DateOnly
                var admissionDate = new DateOnly(year, 9, 1);

                // Check for existing group
                var existingGroup = await _context.StudentGroups
                    .FirstOrDefaultAsync(g => g.GroupCode == row.GroupCode && g.AdmissionYear == admissionDate);

                if (existingGroup != null)
                {
                    existingGroup.Course = course;
                    existingGroup.EducationalProgramId = ep.IdEducationalProgram;
                    existingGroup.StudyFormId = studyForm.IdStudyForm;
                    existingGroup.IsAccelerated = isAccelerated;
                    existingGroup.Avail = true;
                }
                else
                {
                    var newGroup = new StudentGroup
                    {
                        IdGroup = Guid.NewGuid(),
                        GroupCode = row.GroupCode,
                        Course = course,
                        AdmissionYear = admissionDate,
                        EducationalProgramId = ep.IdEducationalProgram,
                        StudyFormId = studyForm.IdStudyForm,
                        IsAccelerated = isAccelerated,
                        Avail = true
                    };
                    _context.StudentGroups.Add(newGroup);
                }
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing group {row.GroupCode}");
                errorCount++;
            }
        }

        await _context.SaveChangesAsync();
        return $"Group import finished. Success: {successCount}, Errors: {errorCount}";
    }

    public async Task<string> CreateStudentUsersAsync(IFormFile file)
    {
        var excelRows = await _excelService.ExtractStudentsAsync(file);

        int createdCount = 0;
        int errorCount = 0;

        var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");

        foreach (var row in excelRows)
        {
            try
            {
                // Find student by EdboCode
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.EdboCode == row.EdboCode);

                if (student == null) continue;

                // Check if student already has a user
                if (student.UserId != Guid.Empty)
                {
                    // Optionally update email if needed
                    var existingUser = await _context.Users.FindAsync(student.UserId);
                    if (existingUser != null && !string.IsNullOrEmpty(row.Email))
                    {
                        existingUser.Email = row.Email;
                    }
                    continue;
                }

                if (string.IsNullOrEmpty(row.Email))
                {
                    _logger.LogWarning($"No email provided for student {row.EdboCode}");
                    continue;
                }

                // Generate password
                string password = PasswordHelper.GeneratePassword();
                PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);

                var user = new User
                {
                    IdUser = Guid.NewGuid(),
                    Email = row.Email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow,
                    IsFirstLogin = true,
                    Avail = true
                };

                _context.Users.Add(user);
                student.UserId = user.IdUser;

                if (studentRole != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.IdUser,
                        RoleId = studentRole.IdRole
                    });
                }

                await _emailService.SendPasswordEmailAsync(row.Email, password);
                createdCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user for row {row.EdboCode}");
                errorCount++;
            }
        }

        await _context.SaveChangesAsync();
        return $"User creation finished. Created: {createdCount}, Errors: {errorCount}";
    }

    private async Task SaveToDatabaseAsync(GeminiSelectiveDisciplineDto dto, Guid catalogId, bool isFaculty, string originalFilePath)
    {
        // Generate unique name
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFilePath)}";
        var finalPath = Path.Combine(_importedPath, uniqueFileName);

        // Move file
        File.Copy(originalFilePath, finalPath, true);

        // Lookup Department
        Guid departmentId = Guid.Empty;
        if (!string.IsNullOrEmpty(dto.Department))
        {
            var dept = await _context.Departments
                .FirstOrDefaultAsync(d => d.NameDepartment != null && d.NameDepartment.Contains(dto.Department));
            departmentId = dept?.IdDepartment ?? Guid.Empty;
        }

        // Recommended and RecommendedEp preparation
        var recommendedEpIds = new List<Guid>();
        var recommendedJson = new Dictionary<string, List<string>>();

        if (dto.Recommended != null)
        {
            if (dto.Recommended.EducationalPrograms != null && dto.Recommended.EducationalPrograms.Any())
            {
                var eps = await _context.EducationalPrograms
                    .Where(ep => dto.Recommended.EducationalPrograms.Contains(ep.NameEducationalProgram))
                    .ToListAsync();
                foreach (var ep in eps) recommendedEpIds.Add(ep.IdEducationalProgram);
                recommendedJson["EducationalPrograms"] = dto.Recommended.EducationalPrograms;
            }

            if (dto.Recommended.Specialities != null && dto.Recommended.Specialities.Any())
            {
                var specs = await _context.Specialities
                    .Where(s => dto.Recommended.Specialities.Contains(s.Name))
                    .ToListAsync();
                
                var specIds = specs.Select(s => s.IdSpeciality).ToList();
                var epsFromSpecs = await _context.EducationalPrograms
                    .Where(ep => ep.SpecialityId != Guid.Empty && specIds.Contains(ep.SpecialityId))
                    .ToListAsync();
                
                foreach (var ep in epsFromSpecs) recommendedEpIds.Add(ep.IdEducationalProgram);
                recommendedJson["Specialties"] = dto.Recommended.Specialities;
            }

            if (dto.Recommended.Branches != null && dto.Recommended.Branches.Any())
            {
                recommendedJson["Branches"] = dto.Recommended.Branches;
            }
        }

        // Create SelectiveDiscipline
        var discipline = new SelectiveDiscipline
        {
            IdSelectiveDisciplines = Guid.NewGuid(),
            NameSelectiveDisciplines = dto.NameSelectiveDisciplines,
            CodeSelectiveDisciplines = dto.CodeSelectiveDisciplines,
            IsFaculty = isFaculty,
            MinCountPeople = dto.MinCountPeople,
            MaxCountPeople = dto.MaxCountPeople,
            IsEven = dto.IsEven,
            DegreeLevelId = dto.DegreeLevelId,
            CatalogId = catalogId,
            ApprovalStatusId = (await _context.Approvals.FirstOrDefaultAsync(sf => sf.ApprobalLevel == 1))?.IdApproval ?? Guid.Empty,
            TypeOfControlId = (await _context.TypeOfControls.FirstOrDefaultAsync(tc => tc.Type.ToLower() == "���������������� ����"))?.IdTypeOfControl ?? Guid.Empty,
            DepartmentId = departmentId,
            NameDock = uniqueFileName,
            Courses = dto.Courses,
            NeedFix = dto.NeedFix,
            RecommendedEp = recommendedEpIds
        };

        // Teachers Binding and JSON preparation
        var teachersJson = new List<object>();
        if (dto.Teachers != null && dto.Teachers.Any())
        {
            foreach (var teacherName in dto.Teachers)
            {
                var admin = await _context.AdminsPersonals
                    .FirstOrDefaultAsync(a => a.NameAdmin != null && a.NameAdmin.Contains(teacherName));
                
                if (admin != null)
                {
                    teachersJson.Add(new { Id = admin.IdAdmins, Name = admin.NameAdmin });
                    _context.BindTeachersSelectives.Add(new BindTeachersSelective
                    {
                        IdBindTeacherSelective = Guid.NewGuid(),
                        AdminId = admin.IdAdmins,
                        SelectiveDisciplinesId = discipline.IdSelectiveDisciplines,
                        IsHead = true
                    });
                }
                else
                {
                    // If teacher not found in DB, just add the name
                    teachersJson.Add(new { Id = Guid.Empty, Name = teacherName });
                }
            }
        }

        // Create SelectiveDetail
        var detail = new SelectiveDetail
        {
            IdSelectiveDetails = discipline.IdSelectiveDisciplines,
            NameSelectiveDisciplinesEng = dto.NameSelectiveDisciplinesEng,
            Language = dto.Language,
            Prerequisites = dto.Prerequisites,
            WhyInterestingDetermination = dto.WhyInterestingDetermination,
            Provision = dto.Provision,
            UsingIrl = dto.UsingIrl,
            ResultEducation = dto.ResultEducation,
            DisciplineTopics = dto.DisciplineTopics,
            TypesOfTraining = dto.TypesOfTraining,
            Recommended = recommendedJson.Any() ? JsonSerializer.Serialize(recommendedJson) : null,
            Teachers = teachersJson.Any() ? JsonSerializer.Serialize(teachersJson) : null
        };

        _context.SelectiveDisciplines.Add(discipline);
        _context.SelectiveDetails.Add(detail);

        await _context.SaveChangesAsync();
    }

    private void MoveToError(string filePath, string errorMessage)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var errorFilePath = Path.Combine(_errorsPath, $"{Guid.NewGuid()}_{fileName}");
            File.Copy(filePath, errorFilePath);
            
            var logPath = Path.Combine(_errorsPath, "import_errors.log");
            File.AppendAllText(logPath, $"[{DateTime.Now}] {fileName}: {errorMessage}\n");
        }
        catch (Exception) { /* Ignore log errors */ }
    }

    public async Task<(byte[] content, string fileName)> GetSelectiveDisciplineFileAsync(string fileName)
    {
        var filePath = Path.Combine(_importedPath, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found in imported directory.");
        }

        return (await File.ReadAllBytesAsync(filePath), fileName);
    }
}
