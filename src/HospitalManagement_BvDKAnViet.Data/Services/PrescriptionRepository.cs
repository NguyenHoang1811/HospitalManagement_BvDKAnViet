using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement_BvDKAnViet.Data.Services
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext _db;

        public PrescriptionRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            return await _db.Prescriptions
                            .AsNoTracking()
                            .Include(p => p.Medicine)
                            .Include(p => p.MedicalRecord)
                            .OrderByDescending(p => p.PrescriptionId)
                            .ToListAsync();
        }

        public async Task<Prescription?> GetByIdAsync(int id)
        {
            return await _db.Prescriptions
                            .AsNoTracking()
                            .Include(p => p.Medicine)
                            .Include(p => p.MedicalRecord)
                            .FirstOrDefaultAsync(p => p.PrescriptionId == id);
        }

        public async Task<Prescription> AddAsync(Prescription prescription)
        {
            var entry = await _db.Prescriptions.AddAsync(prescription);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(Prescription prescription)
        {
            var existing = await _db.Prescriptions.FindAsync(prescription.PrescriptionId);
            if (existing is null) return false;

            existing.RecordId = prescription.RecordId;
            existing.MedicineId = prescription.MedicineId;
            existing.Dosage = prescription.Dosage;
            existing.Instructions = prescription.Instructions;

            _db.Prescriptions.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Prescriptions.FindAsync(id);
            if (existing is null) return false;

            _db.Prescriptions.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
