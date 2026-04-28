using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO
{
    public class UpdateUserDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
    }
}
