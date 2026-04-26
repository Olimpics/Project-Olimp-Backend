using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Models;

namespace OlimpBack.Controllers
{
    // Controllers/UserController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUsers()
        {
            var dtos = await _context.Users
                .ProjectTo<UserRoleDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(dtos);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoleDto>> GetUser(int id)
        {
            var dto = await _context.Users
                .Where(u => u.IdUser == id)
                .ProjectTo<UserRoleDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _mapper.Map(dto, user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            { 
                throw;
            }

            return NoContent();
        }
    }

}