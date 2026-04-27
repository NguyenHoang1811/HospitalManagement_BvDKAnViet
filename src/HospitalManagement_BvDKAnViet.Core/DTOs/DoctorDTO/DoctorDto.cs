namespace HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = null!;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? DepartmentId { get; set; }
        public string? DoctorImage { get; set; }
    }
}