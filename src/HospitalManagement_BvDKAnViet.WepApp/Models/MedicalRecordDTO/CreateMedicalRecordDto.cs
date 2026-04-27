using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.MedicalRecordDTO
{
    public class CreateMedicalRecordDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn bệnh nhân")]
        public int PatientId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn bác sĩ")]
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
