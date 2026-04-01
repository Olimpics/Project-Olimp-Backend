using Microsoft.EntityFrameworkCore;
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
                return existingUser.IdUsers;

            PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);

            var user = new Models.User
            {
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = roleId,
                IsFirstLogin = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.IdUsers;
        }
    }
}
