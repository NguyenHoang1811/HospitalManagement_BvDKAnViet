using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO

{
    public class LoginRequestDto
    {
        [Required, StringLength(100)]
        public string Username { get; set; } = null!;

        [Required, StringLength(200)]
        public string Password { get; set; } = null!;

    }
}
