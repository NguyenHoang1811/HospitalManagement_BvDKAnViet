using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.MedicalRecordDTO
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

        [StringLength(50)]
        public string? MedicalRecordStatus { get; set; }

        [StringLength(250)]
        public string? Attachment { get; set; }

        [StringLength(2000)]
        public string? Result { get; set; }

        [StringLength(2000)]
        public string? Note { get; set; }
    }
}
