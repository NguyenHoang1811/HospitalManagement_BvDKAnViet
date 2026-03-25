namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public ICollection<User>? Users { get; set; }
    }
}