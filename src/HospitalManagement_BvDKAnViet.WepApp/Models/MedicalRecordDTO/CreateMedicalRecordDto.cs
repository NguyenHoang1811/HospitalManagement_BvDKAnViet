using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.MedicalRecordDTO
{
    public class CreateMedicalRecordDto
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
