using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.PrescriptionDTO
{
    public class UpdatePrescriptionDto
    {
        [Required]
        public int RecordId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [StringLength(200)]
        public string? Dosage { get; set; }

        [StringLength(2000)]
        public string? Instructions { get; set; }
    }
}
