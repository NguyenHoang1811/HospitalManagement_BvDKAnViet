using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement_BvDKAnViet.Data.Services
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _db;

        public RoleRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _db.Roles
                            .AsNoTracking()
                            .OrderBy(r => r.RoleName)
                            .ToListAsync();
        }
    }
}
