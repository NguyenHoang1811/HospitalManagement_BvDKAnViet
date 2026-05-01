using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServices
{
    public interface IKidneyPredictionRepository
    {
        Task<KidneyPrediction> AddAsync(KidneyPrediction prediction);
        Task<IEnumerable<KidneyPrediction>> GetByPatientIdAsync(int patientId);
        Task<KidneyPrediction?> GetByIdAsync(int id);
        Task<IEnumerable<KidneyPrediction>> GetByDoctorIdAsync(int doctorId);
    }
}