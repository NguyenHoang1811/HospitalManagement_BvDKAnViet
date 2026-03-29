using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using HospitalManagement_BvDKAnViet.Data.Context;

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
    }
}