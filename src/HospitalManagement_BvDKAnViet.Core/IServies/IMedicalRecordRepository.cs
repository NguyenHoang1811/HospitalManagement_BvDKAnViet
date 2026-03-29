using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServies
{
    public interface IMedicalRecordRepository
    {
        Task<IEnumerable<MedicalRecord>> GetAllAsync();
        Task<MedicalRecord?> GetByIdAsync(int id);
        Task<MedicalRecord> AddAsync(MedicalRecord record);
        Task<bool> UpdateAsync(MedicalRecord record);
        Task<bool> DeleteAsync(int id);
    }
}