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
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
                return NotFound("This user doesn't exist");

            if (user.Password != Password)
                return BadRequest("Incorrect password");

            // Get user's permissions
            var permissions = await _context.BindRolePermissions
                .Include(b => b.Permission)
                .Where(b => b.RoleId == user.RoleId)
                .Select(b => new PermissionDto
                {
                    TypePermission = b.Permission.TypePermission,
                    TableName = b.Permission.TableName
                })
                .ToListAsync();

            if (user.Role.NameRole == "Administrator")
            {
                var admin = await _context.AdminsPersonals
                    .Include(a => a.Faculty)
                    .FirstOrDefaultAsync(a => a.UserId == user.IdUsers);

                if (admin == null)
                    return NotFound("Admin profile not found");

                var response = new LoginResponseDto
                {
                    Id = admin.IdAdmins,
                    UserId = admin.UserId,
                    RoleId = user.RoleId,
                    Name = admin.NameAdmin,
                    NameFaculty = admin.Faculty?.NameFaculty,
                    Permissions = permissions
                };

                return Ok(response);
            }
            else
            {
                var student = await _context.Students
                    .Include(s => s.Faculty)
                    .Include(s => s.EducationalProgram)
                    .FirstOrDefaultAsync(s => s.UserId == user.IdUsers);

                if (student == null)
                    return NotFound("This student doesn't exist");

                var response = _mapper.Map<LoginResponseDto>(student);
                response.Permissions = permissions;
                return Ok(response);
            }
        }
    }
} 