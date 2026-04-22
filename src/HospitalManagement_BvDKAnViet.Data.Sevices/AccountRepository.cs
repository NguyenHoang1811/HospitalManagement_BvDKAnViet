using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement_BvDKAnViet.Data.Sevices
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;
        private const int SaltSize = 16; // 128-bit
        private const int HashSize = 32; // 256-bit
        private const int DefaultIterations = 100_000;

        public AccountRepository(ApplicationDbContext db) => _db = db;

        public async Task<User?> Account_Login(LoginRequestDto request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            var user = await _db.Users
                                .Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user is null) return null;

            return VerifyPassword(request.Password, user.Password) ? user : null;
        }

        // Helper: create password hash string in format: {iterations}.{saltBase64}.{hashBase64}
        public static string CreatePasswordHash(string password, int iterations = DefaultIterations)
        {
            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        // Helper: verify password against stored hash (supports legacy plain-text fallback)
        public static bool VerifyPassword(string password, string? storedPassword)
        {
            if (string.IsNullOrEmpty(storedPassword))
                return false;

            var parts = storedPassword.Split('.');
            if (parts.Length == 3 && int.TryParse(parts[0], out var iterations))
            {
                try
                {
                    var salt = Convert.FromBase64String(parts[1]);
                    var expectedHash = Convert.FromBase64String(parts[2]);

                    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                    var actualHash = pbkdf2.GetBytes(expectedHash.Length);

                    return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
                }
                catch
                {
                    return false;
                }
            }

            // Fallback plain-text comparison (for backward compatibility). Migrate existing accounts to hashed format.
            return storedPassword == password;
        }
    }
}