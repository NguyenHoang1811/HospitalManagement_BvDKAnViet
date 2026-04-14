using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.MedicineDTO
{
    public class UpdateMedicineDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
