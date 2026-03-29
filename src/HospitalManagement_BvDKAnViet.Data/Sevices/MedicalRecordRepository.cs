using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using HospitalManagement_BvDKAnViet.Data.Context;

namespace HospitalManagement_BvDKAnViet.Data.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly ApplicationDbContext _db;

        public MedicalRecordRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _db.MedicalRecords
                            .AsNoTracking()
                            .Include(m => m.Patient)
                            .Include(m => m.Doctor)
                            .Include(m => m.Prescriptions)
                                .ThenInclude(p => p.Medicine)
                            .OrderByDescending(m => m.CreatedDate)
                            .ToListAsync();
        }

        public async Task<MedicalRecord?> GetByIdAsync(int id)
        {
            return await _db.MedicalRecords
                            .AsNoTracking()
                            .Include(m => m.Patient)
                            .Include(m => m.Doctor)
                            .Include(m => m.Prescriptions)
                                .ThenInclude(p => p.Medicine)
                            .FirstOrDefaultAsync(m => m.RecordId == id);
        }

        public async Task<MedicalRecord> AddAsync(MedicalRecord record)
        {
            var entry = await _db.MedicalRecords.AddAsync(record);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(MedicalRecord record)
        {
            var existing = await _db.MedicalRecords.FindAsync(record.RecordId);
            if (existing is null) return false;

            // update scalar properties only
            existing.PatientId = record.PatientId;
            existing.DoctorId = record.DoctorId;
            existing.Symptoms = record.Symptoms;
            existing.Diagnosis = record.Diagnosis;
            existing.Treatment = record.Treatment;
            // preserve CreatedDate

            _db.MedicalRecords.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.MedicalRecords.FindAsync(id);
            if (existing is null) return false;

            _db.MedicalRecords.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}