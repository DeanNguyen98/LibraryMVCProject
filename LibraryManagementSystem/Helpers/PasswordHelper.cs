using System;
using System.Security.Cryptography;

namespace LibraryManagementSystem.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // random 16-byte salt so two identical passwords never hash the same way
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000);
            byte[] hash = pbkdf2.GetBytes(32);

            // prepend the salt so we can recover it during verification
            byte[] combined = new byte[48];
            Array.Copy(salt, 0, combined, 0, 16);
            Array.Copy(hash, 0, combined, 16, 32);

            return Convert.ToBase64String(combined);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // first 16 bytes are the salt we stored on registration
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000);
            byte[] hash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;

            return true;
        }
    }
}
