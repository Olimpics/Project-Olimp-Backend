using System.ComponentModel.DataAnnotations;

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
    }

}
