using System.Security.Cryptography;
using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement_BvDKAnViet.Data.Services
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

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users
                            .AsNoTracking()
                            .Include(u => u.Role)
                            .OrderBy(u => u.Username)
                            .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users
                            .AsNoTracking()
                            .Include(u => u.Role)
                            .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> CreateAsync(CreateUserDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            // check username uniqueness
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return null;

            var user = new User
            {
                Username = dto.Username,
                Password = CreatePasswordHash(dto.Password),
                RoleId = dto.RoleId,
                RefreshToken = null,
                ExpriredTime = null
            };

            var entry = await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(UpdateUserDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var existing = await _db.Users.FindAsync(dto.UserId);
            if (existing is null) return false;

            // Update username if provided in DTO (some DTO variants may include it)
            var usernameProp = dto.GetType().GetProperty("Username");
            if (usernameProp != null)
            {
                var uname = usernameProp.GetValue(dto) as string;
                if (!string.IsNullOrWhiteSpace(uname) && uname != existing.Username)
                {
                    // check uniqueness
                    if (await _db.Users.AnyAsync(u => u.Username == uname && u.UserId != existing.UserId))
                        return false; // username conflict
                    existing.Username = uname;
                }
            }

            existing.RoleId = dto.RoleId;

            // If DTO contains NewPassword, update hashed password here (but require separate flow to confirm)
            var newPassProp = dto.GetType().GetProperty("NewPassword");
            if (newPassProp != null)
            {
                var newPass = newPassProp.GetValue(dto) as string;
                if (!string.IsNullOrWhiteSpace(newPass))
                {
                    existing.Password = CreatePasswordHash(newPass);
                }
            }

            _db.Users.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Users.FindAsync(id);
            if (existing is null) return false;

            _db.Users.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var user = await _db.Users.FindAsync(dto.UserId);
            if (user is null) return false;

            // verify current password
            if (!VerifyPassword(dto.CurrentPassword, user.Password))
                return false;

            user.Password = CreatePasswordHash(dto.NewPassword);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUsernameTakenAsync(string username, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            var query = _db.Users.AsQueryable();
            if (excludeUserId.HasValue)
                query = query.Where(u => u.UserId != excludeUserId.Value);

            return await query.AnyAsync(u => u.Username == username);
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