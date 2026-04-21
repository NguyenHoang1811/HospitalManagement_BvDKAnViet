using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.MedicineDTO
{
    public class CreateMedicineDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
