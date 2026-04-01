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
    }
}