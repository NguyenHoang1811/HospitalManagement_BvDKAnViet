using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServies
{
    public interface IAccountRepository
    {
        Task<User> Account_Login(LoginRequestDto request);
    }
}
