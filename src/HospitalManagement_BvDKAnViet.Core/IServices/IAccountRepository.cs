using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServices
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Authenticate account by username/password. Returns User with Role on success, null on failure.
        /// </summary>
        Task<User?> Account_Login(LoginRequestDto request);

        /// <summary>
        /// Get all users.
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Get user by id.
        /// </summary>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Create new user. Returns created user or null if username already exists.
        /// </summary>
        Task<User?> CreateAsync(CreateUserDto dto);

        /// <summary>
        /// Update existing user (role / username as provided). Returns true on success.
        /// </summary>
        Task<bool> UpdateAsync(UpdateUserDto dto);

        /// <summary>
        /// Delete user by id. Returns true on success.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Change password for user. Returns true on success.
        /// </summary>
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);

        /// <summary>
        /// Check whether username is already taken. If excludeUserId provided, exclude that user from check.
        /// </summary>
        Task<bool> IsUsernameTakenAsync(string username, int? excludeUserId = null);
    }
}
