namespace HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
