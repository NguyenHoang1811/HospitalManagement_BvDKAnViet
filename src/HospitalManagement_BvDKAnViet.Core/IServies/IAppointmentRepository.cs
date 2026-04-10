using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Core.IServies
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
    }
}