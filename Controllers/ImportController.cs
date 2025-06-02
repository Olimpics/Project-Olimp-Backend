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
        public async Task<IActionResult> ImportFile([FromForm] IFormFile file, [FromForm] string entityType)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File not selected or empty" });

            if (file.Length > MaxFileSize)
                return BadRequest(new { message = "File exceeds maximum size (10 MB)" });

            if (string.IsNullOrWhiteSpace(entityType))
                return BadRequest(new { message = "Entity type not specified" });

            try
            {
                var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Очистка имени и добавление Guid
                var ext = Path.GetExtension(file.FileName);
                var safeName = Path.GetFileNameWithoutExtension(file.FileName);
                safeName = Regex.Replace(safeName, @"[^a-zA-Z0-9_\-]", "_");

                var fileName = $"{safeName}_{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Отправка запроса в микросервис
                var client = _httpClientFactory.CreateClient();
                var request = new
                {
                    entityType = entityType,
                    fileName = fileName
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:5001/api/parse", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(502, new { message = "Error when referencing the server", details = responseContent });
                }

                var parsed = JsonSerializer.Deserialize<ParseResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed == null)
                    return BadRequest(new { message = "Response from service not recognized" });

                if (parsed.Errors?.Any() == true)
                    return BadRequest(new { message = "Parsing error", errors = parsed.Errors });

                if (parsed.Rows.Count > 20)
                    return Ok(new { message = "Parsing is successful. Data more than 20 lines" });

                return Ok(new { message = "Parsing successful", data = parsed.Rows });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

    }
}
