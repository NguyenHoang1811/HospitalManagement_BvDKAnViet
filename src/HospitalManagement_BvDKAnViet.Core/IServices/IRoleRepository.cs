using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServices
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
    }
}
