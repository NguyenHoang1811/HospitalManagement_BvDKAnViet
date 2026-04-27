using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO
{
    public class ChangePasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}