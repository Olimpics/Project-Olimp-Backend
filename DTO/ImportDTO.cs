using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OlimpBack.DTO
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        public string TableName { get; set; } = null!;

        [Required]
        public bool IsCreate { get; set; }

        [Required]
        public int? Limit { get; set; }
    }

    public class ExportRequestDto
    {
        [Required]
        public string TableName { get; set; } = null!;
        public JsonElement? Request { get; set; } // Optional JSON request for filtering or options
    }
}
