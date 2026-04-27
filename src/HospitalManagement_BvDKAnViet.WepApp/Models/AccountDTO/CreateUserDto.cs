using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO;
public class CreateUserDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [StringLength(100)]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(200, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn vai trò")]
    [Display(Name = "Vai trò")]
    public int RoleId { get; set; }
}