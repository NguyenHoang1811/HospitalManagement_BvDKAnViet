namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public ICollection<Doctor>? Doctors { get; set; }
    }
}