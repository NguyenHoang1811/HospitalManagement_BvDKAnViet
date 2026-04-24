using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Data.Context;
using HospitalManagement_BvDKAnViet.Core.IServices;  


namespace HospitalManagement_BvDKAnViet.Data.Repositories
{

    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _db;

        public PatientRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _db.Patients
                            .AsNoTracking()
                            .OrderByDescending(p => p.CreatedAt)
                            .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _db.Patients
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            var entry = await _db.Patients.AddAsync(patient);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            var existing = await _db.Patients.FindAsync(patient.PatientId);
            if (existing is null) return false;

            // Update scalar properties explicitly to avoid overwriting navigation properties unintentionally
            existing.Name = patient.Name;
            existing.DateOfBirth = patient.DateOfBirth;
            existing.Gender = patient.Gender;
            existing.Phone = patient.Phone;
            existing.Address = patient.Address;
            existing.MedicalHistory = patient.MedicalHistory;
            // Do not update CreatedAt to preserve original creation time

            _db.Patients.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Patients.FindAsync(id);
            if (existing is null) return false;

            _db.Patients.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}