using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.PatientDTO
{
    public class UpdatePatientDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(2000)]
        public string? MedicalHistory { get; set; }
    }
}
