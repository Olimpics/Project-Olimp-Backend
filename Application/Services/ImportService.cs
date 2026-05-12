using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;
using System.IO.Compression;
using System.Text.Json;

namespace OlimpBack.Application.Services;

public class ImportService : IImportService
{
    private readonly IWordProcessingService _wordService;
    private readonly IGeminiService _geminiService;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImportService> _logger;

    private readonly string _uploadPath;
    private readonly string _importedPath;
    private readonly string _errorsPath;

    public ImportService(
        IWordProcessingService wordService,
        IGeminiService geminiService,
        AppDbContext context,
        IWebHostEnvironment environment,
        ILogger<ImportService> logger)
    {
        _wordService = wordService;
        _geminiService = geminiService;
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

    private async Task SaveToDatabaseAsync(GeminiSelectiveDisciplineDto dto, int catalogId, bool isFaculty, string originalFilePath)
    {
        // Generate unique name
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFilePath)}";
        var finalPath = Path.Combine(_importedPath, uniqueFileName);

        // Move file
        File.Copy(originalFilePath, finalPath, true);

        // Lookup Department
        int? departmentId = null;
        if (!string.IsNullOrEmpty(dto.Department))
        {
            var dept = await _context.Departments
                .FirstOrDefaultAsync(d => d.NameDepartment != null && d.NameDepartment.Contains(dto.Department));
            departmentId = dept?.IdDepartment;
        }

        // Recommended and RecommendedEp preparation
        var recommendedEpIds = new HashSet<int>();
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
                    .Where(ep => ep.SpecialityId.HasValue && specIds.Contains(ep.SpecialityId.Value))
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
            IdSelectiveDisciplines = await GenerateNextDisciplineId(),
            NameSelectiveDisciplines = dto.NameSelectiveDisciplines,
            CodeSelectiveDisciplines = dto.CodeSelectiveDisciplines,
            IsFaculty = isFaculty ? 1 : 0,
            MinCountPeople = dto.MinCountPeople,
            MaxCountPeople = dto.MaxCountPeople,
            IsEven = dto.IsEven != null ? new System.Collections.BitArray(new[] { dto.IsEven == 1 }) : null,
            DegreeLevelId = dto.DegreeLevelId,
            CatalogId = catalogId,
            ApprovalStatusId = 4, // Approved/Published as per user request
            TypeOfControlId = 2,
            DepartmentId = departmentId,
            NameDock = uniqueFileName,
            Courses = dto.Courses,
            NeedFix = new System.Collections.BitArray(new[] { dto.NeedFix }),
            RecommendedEp = recommendedEpIds.ToList()
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
                        IdBindTeacherSelective = await GenerateNextBindTeacherId(),
                        AdminId = admin.IdAdmins,
                        SelectiveDisciplinesId = discipline.IdSelectiveDisciplines,
                        IsHead = new System.Collections.BitArray(new[] { true }) // Defaulting to head for simplicity
                    });
                }
                else
                {
                    // If teacher not found in DB, just add the name
                    teachersJson.Add(new { Id = 0, Name = teacherName });
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
            DisciplineTopics = dto.DisciplineTopics != null ? JsonSerializer.Serialize(dto.DisciplineTopics) : null,
            TypesOfTraining = dto.TypesOfTraining,
            Recommended = recommendedJson.Any() ? JsonSerializer.Serialize(recommendedJson) : null,
            Teachers = teachersJson.Any() ? JsonSerializer.Serialize(teachersJson) : null
        };

        _context.SelectiveDisciplines.Add(discipline);
        _context.SelectiveDetails.Add(detail);

        await _context.SaveChangesAsync();
    }

    private async Task<int> GenerateNextDisciplineId()
    {
        var maxId = await _context.SelectiveDisciplines.MaxAsync(x => (int?)x.IdSelectiveDisciplines) ?? 0;
        return maxId + 1;
    }

    private async Task<int> GenerateNextBindTeacherId()
    {
        var maxId = await _context.BindTeachersSelectives.MaxAsync(x => (int?)x.IdBindTeacherSelective) ?? 0;
        return maxId + 1;
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
