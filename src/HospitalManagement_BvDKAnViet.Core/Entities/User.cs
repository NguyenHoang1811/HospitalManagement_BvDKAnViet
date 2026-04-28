namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!; // store hashed password
        public int RoleId { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? ExpriredTime { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }

        // Navigation
        public Role? Role { get; set; }
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}