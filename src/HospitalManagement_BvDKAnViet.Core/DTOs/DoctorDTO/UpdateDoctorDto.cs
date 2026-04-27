using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO
{
    public class UpdateDoctorDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? Specialty { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress, StringLength(200)]
        public string? Email { get; set; }

        public int? DepartmentId { get; set; }

        public string? DoctorImage { get; set; }
    }
}