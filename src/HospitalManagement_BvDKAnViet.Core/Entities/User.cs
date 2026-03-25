namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!; // store hashed password
        public int RoleId { get; set; }

        // Navigation
        public Role? Role { get; set; }
    }
}