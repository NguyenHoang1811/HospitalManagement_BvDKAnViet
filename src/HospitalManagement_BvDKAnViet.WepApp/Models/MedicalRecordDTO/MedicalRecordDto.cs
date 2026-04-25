namespace HospitalManagement_BvDKAnViet.WepApp.Models.MedicalRecordDTO
{
    public class MedicalRecordDto
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? MedicalRecordStatus { get; set; }
        public string? Attachment { get; set; }
        public string? Result { get; set; }
        public string? Note { get; set; }

        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
    }
}
