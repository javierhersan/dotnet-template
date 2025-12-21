using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        public string? Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? HashedPassword { get; set; } 

        public User(string username, string email)
        {
            Username = username;
            Email = email;
        }

        public void AddPassword(string password)
        {
            HashedPassword = HashPassword(password);
        }

        public bool ValidatePassword(string password)
        {
            if (HashedPassword == null)
                return false;
            return VerifyPassword(password, HashedPassword);
        }

        /// <summary>
        /// Hashes a plain text password using SHA256.
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verifies a plain text password against a hashed password.
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedPasswordInput = HashPassword(password);
            return hashedPasswordInput == hashedPassword;
        }
    }
}