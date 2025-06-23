using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public ExportController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        [HttpGet("export-to-file")]
        public async Task<IActionResult> ExportToFile([FromQuery] string tableName, [FromQuery] string stringRequest)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(stringRequest))
                return BadRequest(new { message = "Missing tableName or stringRequest" });

            string endpoint = tableName switch
            {
                "Виборчі дисціпліни" => "http://localhost:5001/api/export/disciplines",
                "Студенти" => "http://localhost:5001/api/export/students",
                "Спеціальності" => "http://localhost:5001/api/export/specialities",
                "Групи" => "http://localhost:5001/api/export/groups",
                _ => ""
            };

            if (string.IsNullOrEmpty(endpoint))
                return BadRequest(new { message = $"Unknown table: {tableName}" });

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(stringRequest);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode(502, new { message = "Error calling provided URL", details = error });
                }
                var jsonString = await response.Content.ReadAsStringAsync();
                
                using var doc = JsonDocument.Parse(jsonString);
                var formattedJson = JsonSerializer.Serialize(
                    doc.RootElement,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    }
                );

                var safeTableName = Regex.Replace(tableName, @"[^a-zA-Z0-9_\-]", "_");
                var fileName = $"{safeTableName}_{Guid.NewGuid():N}.txt";
                var filePath = "/opt/Project-Olimp-Parser/fastapi-project/export_files";
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                var fullPath = Path.Combine(filePath, fileName);
                await System.IO.File.WriteAllTextAsync(fullPath, formattedJson, Encoding.UTF8);
                var fileUrl = $"{endpoint}/{fileName}";
                return Ok(new { message = "Export successful", fileName });
            }
            catch (JsonException)
            {
                return BadRequest(new { message = "Response from the provided URL is not valid JSON." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest(new { message = "File name is required" });

            var filePath = "/opt/Project-Olimp-Parser/fastapi-project/export_transform_files";

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "File not found" });

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "text/plain", fileName);
        }
    }
}
