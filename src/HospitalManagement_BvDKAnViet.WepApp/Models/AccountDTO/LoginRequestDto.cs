using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO

{
    public class LoginRequestDto
    {
        [StringLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string Username { get; set; } = null!;

        [StringLength(200)]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; } = null!;

    }
}
