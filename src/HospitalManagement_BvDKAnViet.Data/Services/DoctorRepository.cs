using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Data.Context;
using HospitalManagement_BvDKAnViet.Core.IServices;

namespace HospitalManagement_BvDKAnViet.Data.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _db;

        public DoctorRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _db.Doctors
                            .AsNoTracking()
                            .OrderBy(d => d.Name)
                            .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _db.Doctors
                            .AsNoTracking()
                            .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            var entry = await _db.Doctors.AddAsync(doctor);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            var existing = await _db.Doctors.FindAsync(doctor.DoctorId);
            if (existing is null) return false;

            existing.Name = doctor.Name;
            existing.Specialty = doctor.Specialty;
            existing.Phone = doctor.Phone;
            existing.Email = doctor.Email;
            existing.DepartmentId = doctor.DepartmentId;
            existing.DoctorImage = doctor.DoctorImage;

            _db.Doctors.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Doctors.FindAsync(id);
            if (existing is null) return false;

            _db.Doctors.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}