namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = null!;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? DepartmentId { get; set; }
        public string? DoctorImage { get; set; }

        // Navigation
        public Department? Department { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
    }
}