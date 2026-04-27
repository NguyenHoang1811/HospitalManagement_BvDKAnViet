using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO
{
    public class ChangePasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required, StringLength(200, MinimumLength = 1)]
        public string CurrentPassword { get; set; } = null!;

        [Required, StringLength(200, MinimumLength = 6)]
        public string NewPassword { get; set; } = null!;

        [Required, Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
