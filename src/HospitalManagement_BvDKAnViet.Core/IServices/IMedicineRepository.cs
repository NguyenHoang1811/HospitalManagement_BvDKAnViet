using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServices
{
    public interface IMedicineRepository
    {
        Task<IEnumerable<Medicine>> GetAllAsync();
        Task<Medicine?> GetByIdAsync(int id);
        Task<Medicine> AddAsync(Medicine medicine);
        Task<bool> UpdateAsync(Medicine medicine);
        Task<bool> DeleteAsync(int id);
    }
}
