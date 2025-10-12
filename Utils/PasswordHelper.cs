using System.Security.Cryptography;

namespace OlimpBack.Utils
{
    public static class PasswordHelper
    {
        // PBKDF2 params
        private const int SaltSize = 16; // bytes
        private const int KeySize = 32; // bytes
        private const int Iterations = 100_000;

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var rng = RandomNumberGenerator.Create();
            passwordSalt = new byte[SaltSize];
            rng.GetBytes(passwordSalt);

            using var deriveBytes = new Rfc2898DeriveBytes(password, passwordSalt, Iterations, HashAlgorithmName.SHA256);
            passwordHash = deriveBytes.GetBytes(KeySize);
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, storedSalt, Iterations, HashAlgorithmName.SHA256);
            var computed = deriveBytes.GetBytes(KeySize);
            return CryptographicOperations.FixedTimeEquals(computed, storedHash);
        }

        // Генератор паролів (опційно, керується конфігом)
        public static string GeneratePassword(int length = 12)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specials = "!@#$%^&*()-_+=[]{}|;:,.<>?";

            var all = upper + lower + digits + specials;
            var rnd = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rnd.GetBytes(bytes);
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = all[bytes[i] % all.Length];
            }
            return new string(chars);
        }

        // Перевірка політики пароля згідно з вашими вимогами:
        // - допустима довжина групи = 2 літери
        // - обмеження на кількість таких груп = 1
        // - відбраковувати групи > 2 (тобто "aaa" неприпустимо)
        public static (bool IsValid, string Error) ValidatePasswordPolicy(string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, "Password cannot be empty.");

            // Можна додати базові обмеження: мін довжина
            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            int groupsOfTwo = 0;
            int i = 0;
            while (i < password.Length)
            {
                int j = i + 1;
                while (j < password.Length && password[j] == password[i]) j++;

                int groupLen = j - i;
                if (groupLen > 2)
                {
                    return (false, $"Password contains a group of {groupLen} identical characters starting at position {i + 1} — groups longer than 2 are not allowed.");
                }
                if (groupLen == 2)
                {
                    groupsOfTwo++;
                    if (groupsOfTwo > 1)
                        return (false, "Password contains more than one group of two identical characters — only one such group is allowed.");
                }
                i = j;
            }

            // Тут можна додати інші правила (символи, цифри, тощо) залежно від варіанту
            return (true, string.Empty);
        }
    }

}
