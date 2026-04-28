
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO
{
    public class CreateUserDto
    {
        [Required, StringLength(100)]
        public string Username { get; set; } = null!;

        [Required, StringLength(200, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required, Compare(nameof(Password), ErrorMessage = "Confirm password does not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
    }
}
