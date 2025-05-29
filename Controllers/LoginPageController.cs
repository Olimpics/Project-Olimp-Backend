using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginPageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LoginPageController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<LoginResponseDto>> Login([FromQuery] string Email, [FromQuery] string Password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
                return NotFound("This user doesn't exist");

            var student = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.EducationalProgram)
                .FirstOrDefaultAsync(s => s.UserId == user.IdUsers);

            if (student == null)
                return NotFound("This student doesn't exist");

            if (student.User.Password != Password)
                return BadRequest("Incorrect password");

            var response = _mapper.Map<LoginResponseDto>(student);
            return Ok(response);
        }

    }
} 