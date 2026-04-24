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
    }
}
