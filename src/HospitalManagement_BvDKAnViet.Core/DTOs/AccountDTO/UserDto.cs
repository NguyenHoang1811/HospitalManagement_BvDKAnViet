namespace HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        
    }
}
