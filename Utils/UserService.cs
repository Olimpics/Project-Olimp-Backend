using Microsoft.EntityFrameworkCore;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Data;

namespace OlimpBack.Utils
{
    public static class UserService
    {
        public static async Task<int> CreateUserForStudent(string studentName, AppDbContext _context)
        {
            
            var email = $"{studentName.Replace(" ", ".").ToLower()}@student.local";
            var password = "default_password";
            int roleId = 1;

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
                return existingUser.IdUser;

            PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);

            var user = new Models.User
            {
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsFirstLogin = new System.Collections.BitArray(1, true),
                CreatedAt = ToPostgresTimestamp(DateTime.UtcNow),
                LastLoginAt = ToPostgresTimestamp(DateTime.UtcNow)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (await _context.Roles.AnyAsync(r => r.IdRole == roleId))
            {
                _context.UserRoles.Add(new Models.UserRole
                {
                    UserId = user.IdUser,
                    RoleId = roleId
                });
                await _context.SaveChangesAsync();
            }

            return user.IdUser;
        }

        private static DateTime ToPostgresTimestamp(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
        }
    }
}
