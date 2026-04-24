using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServices
{
    public interface IJwtService
    {
        string GenerateToken(User user);

        // Persist user's refresh token and expiry to storage
        Task UpdateRefreshToken(User user);
    }
}
