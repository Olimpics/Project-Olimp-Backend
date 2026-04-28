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
                Passwordhash = hash,
                Passwordsalt = salt,
                Isfirstlogin = new System.Collections.BitArray(1, true),
                Createdat = DateTime.UtcNow,
                Lastloginat = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.IdUser;
        }
    }
}
