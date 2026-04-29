using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServices
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> AddAsync(Appointment appointment);
        Task<bool> UpdateAsync(Appointment appointment);
        Task<bool> DeleteAsync(int id);

        //checked bac si va benh nhan co trung lich khong
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateOnly date, TimeOnly time, int? excludeAppointmentId = null);

        Task<bool> IsPatientAvailableAsync(int patientId, DateOnly date, TimeOnly time, int? excludeAppointmentId = null);
        Task<IEnumerable<Appointment>> GetPendingExpiredAsync(DateTime beforeTime);
        Task<bool> BulkUpdateStatusAsync(IEnumerable<int> ids, int status, string statusName);
        Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateOnly date);
    }
}