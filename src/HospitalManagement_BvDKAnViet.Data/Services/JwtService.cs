using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;
namespace HospitalManagement_BvDKAnViet.Data.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public JwtService(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public string GenerateToken(User user)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT SecretKey is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration["Jwt:ValidIssuer"];
            var audience = _configuration["Jwt:ValidAudience"];
            var expiryMinutes = _configuration.GetValue<int?>("Jwt:TokenValidityInMinutes") ?? 60;

            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username)
                    };

            if (user.Role != null && !string.IsNullOrWhiteSpace(user.Role.RoleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleName));
            }

            // ✅ Thêm DoctorId nếu là bác sĩ
            if (user.DoctorId != null)
            {
                claims.Add(new Claim("DoctorId", user.DoctorId.ToString()!));
            }

            // ✅ Thêm PatientId nếu là bệnh nhân
            if (user.PatientId != null)
            {
                claims.Add(new Claim("PatientId", user.PatientId.ToString()!));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task UpdateRefreshToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var existing = await _dbContext.Set<User>().FindAsync(user.UserId);
            if (existing == null)
                throw new InvalidOperationException($"User with id {user.UserId} not found.");

            // Preserve other fields; update refresh token and expiry
            existing.RefreshToken = user.RefreshToken;
            existing.ExpriredTime = user.ExpriredTime;

            _dbContext.Set<User>().Update(existing);
            await _dbContext.SaveChangesAsync();
        }
    }
}
