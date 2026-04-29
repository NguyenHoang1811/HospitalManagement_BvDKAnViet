using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.Enums;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement_BvDKAnViet.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _db;

        public AppointmentRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _db.Appointments
                            .AsNoTracking()
                            .Include(a => a.Patient)
                            .Include(a => a.Doctor)
                            .OrderByDescending(a => a.AppointmentDate)
                            .ThenByDescending(a => a.AppointmentTime)
                            .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _db.Appointments
                            .AsNoTracking()
                            .Include(a => a.Patient)
                            .Include(a => a.Doctor)
                            .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            var entry = await _db.Appointments.AddAsync(appointment);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(Appointment appointment)
        {
            var existing = await _db.Appointments.FindAsync(appointment.AppointmentId);
            if (existing is null) return false;

            existing.PatientId = appointment.PatientId;
            existing.DoctorId = appointment.DoctorId;
            existing.AppointmentDate = appointment.AppointmentDate;
            existing.AppointmentTime = appointment.AppointmentTime;
            existing.Status = appointment.Status;

            _db.Appointments.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Appointments.FindAsync(id);
            if (existing is null) return false;

            _db.Appointments.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        // Tra ve true neu bac si khong co lich hen trung voi lich moi, false neu co trung
        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateOnly date, TimeOnly time, int? excludeAppointmentId = null)
        {
            var query = _db.Appointments
                           .AsNoTracking()
                           .Where(a => a.DoctorId == doctorId
                                    && a.AppointmentDate == date
                                    && a.AppointmentTime == time);

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.AppointmentId != excludeAppointmentId.Value);

            var conflictExists = await query.AnyAsync();
            return !conflictExists;
        }

        //Tra ve true neu khong co cuoc hen trung lich cho benh nhan tai ngay/thoigian do
        public async Task<bool> IsPatientAvailableAsync(int patientId, DateOnly date, TimeOnly time, int? excludeAppointmentId = null)
        {
            var query = _db.Appointments
                           .AsNoTracking()
                           .Where(a => a.PatientId == patientId
                                    && a.AppointmentDate == date
                                    && a.AppointmentTime == time);

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.AppointmentId != excludeAppointmentId.Value);

            var conflictExists = await query.AnyAsync();
            return !conflictExists;
        }

        public async Task<IEnumerable<Appointment>> GetPendingExpiredAsync(DateTime beforeTime)
        {
            return await _db.Appointments
                .Where(a => a.Status == (int)AppointmentStatus.PENDING
                         && a.AppointmentDate.ToDateTime(a.AppointmentTime) < beforeTime)
                .ToListAsync();
        }

        public async Task<bool> BulkUpdateStatusAsync(IEnumerable<int> ids, int status, string statusName)
        {
            var appointments = await _db.Appointments
                .Where(a => ids.Contains(a.AppointmentId))
                .ToListAsync();

            if (!appointments.Any()) return false;

            foreach (var a in appointments)
            {
                a.Status = status;
                a.StatusName = statusName;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateOnly date)
        {
            return await _db.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
                .ToListAsync();
        }
    }
}