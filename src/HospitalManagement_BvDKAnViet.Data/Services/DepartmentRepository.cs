using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;

namespace HospitalManagement_BvDKAnViet.Data.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _db;

        public DepartmentRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _db.Departments
                            .AsNoTracking()
                            .OrderBy(d => d.DepartmentName)
                            .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _db.Departments
                            .AsNoTracking()
                            .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<Department> AddAsync(Department department)
        {
            var entry = await _db.Departments.AddAsync(department);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            var existing = await _db.Departments.FindAsync(department.DepartmentId);
            if (existing is null) return false;

            existing.DepartmentName = department.DepartmentName;

            _db.Departments.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Departments.FindAsync(id);
            if (existing is null) return false;

            _db.Departments.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}