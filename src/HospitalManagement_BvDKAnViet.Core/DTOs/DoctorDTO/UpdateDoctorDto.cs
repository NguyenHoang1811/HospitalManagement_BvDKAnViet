using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO
{
    public class UpdateDoctorDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? Specialty { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public int? DepartmentId { get; set; }
    }
}