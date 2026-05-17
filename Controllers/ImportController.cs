using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;
using OlimpBack.Data;


namespace OlimpBack.Controllers
{
    public class ParseResponse
    {
        public List<Dictionary<string, string>> Rows { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImportService _importService;

        public ImportController(
            AppDbContext context,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment env,
            IImportService importService)
        {
            _context = context;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _env = env;
            _importService = importService;
        }

        [HttpPost("selective-disciplines")]
        [Consumes("multipart/form-data")]
        [RequirePermission(RbacPermissions.DisciplineCreate)]
        public async Task<IActionResult> UploadSelectiveDisciplines([FromForm] SelectiveDisciplineImportRequestDto dto)
        {
            if (dto.Archive == null || dto.Archive.Length == 0)
                return BadRequest("Archive is empty");

            try
            {
                var result = await _importService.ImportSelectiveDisciplinesAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Import failed", error = ex.Message });
            }
        }

        [HttpPost("groups")]
        [Consumes("multipart/form-data")]
        [RequirePermission(RbacPermissions.GroupsCreate)]
        public async Task<IActionResult> UploadGroups(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            try
            {
                var result = await _importService.ImportGroupsAsync(file);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Import failed", error = ex.Message });
            }
        }

        [HttpPost("students")]
        [Consumes("multipart/form-data")]
        [RequirePermission(RbacPermissions.UsersCreate)] // Or suitable permission
        public async Task<IActionResult> UploadStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            try
            {
                var result = await _importService.ImportStudentsAsync(file);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Import failed", error = ex.Message });
            }
        }

        [HttpPost("create-student-users")]
        [RequirePermission(RbacPermissions.UsersCreate)]
        public async Task<IActionResult> CreateStudentUsers(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            try
            {
                var result = await _importService.CreateStudentUsersAsync(file);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "User creation failed", error = ex.Message });
            }
        }

        [HttpGet("selective-disciplines/file/{fileName}")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<IActionResult> GetSelectiveDisciplineFile(string fileName)
        {
            try
            {
                var (content, actualFileName) = await _importService.GetSelectiveDisciplineFileAsync(fileName);
                return File(content, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", actualFileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }

}
