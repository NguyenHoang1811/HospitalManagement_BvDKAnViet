using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.MedicineDTO
{
    public class MedicineDto
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
