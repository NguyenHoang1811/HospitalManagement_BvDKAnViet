using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement_BvDKAnViet.Core.IServies
{
    public interface IMedicineRepository
    {
        Task<IEnumerable<Medicine>> GetAllAsync();
        Task<Medicine?> GetByIdAsync(int id);
        Task<Medicine> AddAsync(Medicine medicine);
        Task<bool> UpdateAsync(Medicine medicine);
        Task<bool> DeleteAsync(int id);
    }
}
