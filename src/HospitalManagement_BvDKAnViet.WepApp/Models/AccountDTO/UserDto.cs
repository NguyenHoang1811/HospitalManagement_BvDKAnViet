namespace HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

    }
}
