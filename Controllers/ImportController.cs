using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;


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

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File not selected or empty" });

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = "File exceeds maximum size (10 MB)" });

            try
            {
                var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                var safeName = Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^a-zA-Z0-9_\-]", "_");
                var fileName = $"{safeName}_{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileType;
                if ((ext == ".xlsx" || ext == ".xls") && file.ContentType.Contains("spreadsheet"))
                    fileType = "xlsx";
                else if (ext == ".pdf" && file.ContentType == "application/pdf")
                    fileType = "pdf";
                else if (ext == ".docx" && file.ContentType.Contains("word"))
                    fileType = "docx";
                else
                    fileType = "unknown";

                string endpoint = tableName switch
                {
                    "Виборчі дисціпліни" => "http://localhost:5001/api/parse/disciplines",
                    "Студенти" => "http://localhost:5001/api/parse/students",
                    "Спеціальності" => "http://localhost:5001/api/parse/specialities",
                    "Групи" => "http://localhost:5001/api/parse/groups",
                    _ => ""
                };

                if (endpoint == null)
                    return BadRequest(new { message = $"Unknown table: {tableName}" });

                var request = new
                {
                    fileName,
                    filePath = fullPath,
                    fileType,
                    isCreate
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { message = "Error when referencing the parser", details = responseContent });
                }

                return Ok(new { message = "Parsing request sent successfully", result = responseContent });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }


    }
}
