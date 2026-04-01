using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindStudentsFavouriteDisciplinesController : ControllerBase
    {
        private readonly IBindStudentsFavouriteDisciplinesService _service;

        public BindStudentsFavouriteDisciplinesController(IBindStudentsFavouriteDisciplinesService service)
        {
            _service = service;
        }

        // GET: api/BindStudentsFavouriteDisciplines/student/5
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<AddDisciplineDto>>> GetFavoriteDisciplinesByStudent(int studentId)
        {
            var disciplines = await _service.GetFavoriteDiciplinesByStudentAsync(studentId);
            
            // Завжди повертаємо 200 OK. Якщо у студента немає дисциплін, повернеться порожній масив [] (це правильна поведінка для REST API)
            return Ok(disciplines);
        }

        [HttpPost]
        public async Task<ActionResult<StudentFavouriteDisciplineDto>> AddFavorite([FromBody] AddFavoriteDisciplineDto dto)
        {
            var (success, statusCode, errorMessage, resultDto) = await _service.AddFavoriteAsync(dto);

            if (!success)
            {
                return StatusCode(statusCode, new { message = errorMessage });
            }

            // Повертаємо статус 201 Created і саму DTOшку
            return StatusCode(StatusCodes.Status201Created, resultDto);
        }

        // Видаляємо за ID самого зв'язку (IdBindAddDisciplines)
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var (success, statusCode, errorMessage) = await _service.RemoveFavoriteAsync(id);

            if (!success)
            {
                return StatusCode(statusCode, new { message = errorMessage });
            }

            return NoContent(); // 204
        }
    }
}