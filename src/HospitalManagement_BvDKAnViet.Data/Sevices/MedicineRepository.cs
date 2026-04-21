using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using HospitalManagement_BvDKAnViet.Data.Context;

namespace HospitalManagement_BvDKAnViet.Data.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly ApplicationDbContext _db;

        public MedicineRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Medicine>> GetAllAsync()
        {
            return await _db.Medicines
                            .AsNoTracking()
                            .Include(m => m.Prescriptions)
                            .OrderBy(m => m.Name)
                            .ToListAsync();
        }

        public async Task<Medicine?> GetByIdAsync(int id)
        {
            return await _db.Medicines
                            .AsNoTracking()
                            .Include(m => m.Prescriptions)
                            .FirstOrDefaultAsync(m => m.MedicineId == id);
        }

        public async Task<Medicine> AddAsync(Medicine medicine)
        {
            var entry = await _db.Medicines.AddAsync(medicine);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(Medicine medicine)
        {
            var existing = await _db.Medicines.FindAsync(medicine.MedicineId);
            if (existing is null) return false;

            existing.Name = medicine.Name;
            existing.Price = medicine.Price;

            _db.Medicines.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Medicines.FindAsync(id);
            if (existing is null) return false;

            _db.Medicines.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}