using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Data;
using OlimpBack.DTO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


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

        public ImportController(AppDbContext context, IMapper mapper, IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportFile([FromForm] FileUploadDto dto)
        {
            var file = dto.File;
            var tableName = dto.TableName;
            var isCreate = dto.IsCreate;
            int? limit = dto.Limit;

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File not selected or empty" });

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = "File exceeds maximum size (10 MB)" });

            try
            {
                var inputFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/input_files";
                if (!Directory.Exists(inputFilesPath))
                    Directory.CreateDirectory(inputFilesPath);

                var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                var safeName = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^a-zA-Z0-9_-]", "_");
                var fileName = $"{safeName}_{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(inputFilesPath, fileName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string endpointBase = tableName switch
                {
                    "Виборчі дисціпліни" => "http://localhost:5001/api/parse/disciplines",
                    "Студенти" => "http://185.237.207.78:9000/api/parse-students",
                    "Спеціальності" => "http://localhost:5001/api/parse/specialities",
                    "Групи" => "http://localhost:5001/api/parse/groups",
                    _ => null
                };

                if (string.IsNullOrWhiteSpace(endpointBase))
                    return BadRequest(new { message = $"Unknown table: {tableName}" });

                var client = _httpClientFactory.CreateClient();
                var requestBody = new { fileName, limit };
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpointBase, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { message = "Error calling parser", details = responseContent });
                }

                return Ok(new
                {
                    message = "Parsing request sent successfully",
                    file = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpGet("preview")]
        public IActionResult PreviewFile([FromQuery] string fileName, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var parsedFilesPath = "/opt/Project-Olimp-Parser/fastapi-project/parsed_json";
                var fullPath = Path.Combine(parsedFilesPath, fileName);

                if (!System.IO.File.Exists(fullPath))
                    return NotFound(new { message = "Parsed file not found" });

                var jsonContent = System.IO.File.ReadAllText(fullPath);

                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);

                if (data == null)
                    return BadRequest(new { message = "Failed to parse JSON content." });

                var totalItems = data.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    currentPage = page,
                    pageSize,
                    totalItems,
                    totalPages,
                    items
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reading file", error = ex.Message });
            }
        }
    }

}
