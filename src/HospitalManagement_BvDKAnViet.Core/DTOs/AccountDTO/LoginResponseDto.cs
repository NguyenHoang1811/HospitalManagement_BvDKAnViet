namespace HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
