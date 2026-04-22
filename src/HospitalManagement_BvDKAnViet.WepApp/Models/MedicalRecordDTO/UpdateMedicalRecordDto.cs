using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.MedicalRecordDTO
{
    public class UpdateMedicalRecordDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [StringLength(2000)]
        public string? Symptoms { get; set; }

        [StringLength(2000)]
        public string? Diagnosis { get; set; }

        [StringLength(2000)]
        public string? Treatment { get; set; }
    }
}
