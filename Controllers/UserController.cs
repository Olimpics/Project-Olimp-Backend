using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
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
                .Where(u => u.IdUsers == id)
                .ProjectTo<UserRoleDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserRoleDto>> CreateUser(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            user.LastLoginAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var resultDto = await _context.Users
                .Where(u => u.IdUsers == user.IdUsers)
                .ProjectTo<UserRoleDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.IdUsers }, resultDto);
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
                if (!UserExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.IdUsers == id);
        }
    }

}