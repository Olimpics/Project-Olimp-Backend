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
        public async Task<ActionResult<LoginResponseDto>> Login([FromQuery] LoginRequestDto request)
        {
            var user = await _context.Users
                .Include(u => u.Students)
                    .ThenInclude(s => s.Faculty)
                .Include(u => u.Students)
                    .ThenInclude(s => s.EducationalProgram)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return NotFound("This user isn't exist");
            }

            if (user.Password != request.Password)
            {
                return BadRequest("Incorrect password");
            }

            var response = _mapper.Map<LoginResponseDto>(user);
            return Ok(response);
        }
    }
} 