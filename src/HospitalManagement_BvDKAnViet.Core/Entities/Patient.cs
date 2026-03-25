using System;

namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string Name { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? MedicalHistory { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public ICollection<KidneyPrediction>? KidneyPredictions { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
    }
}