using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class MedicalRecord
    {
        [Key]
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

        // Navigation
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public ICollection<Prescription>? Prescriptions { get; set; }
    }
}