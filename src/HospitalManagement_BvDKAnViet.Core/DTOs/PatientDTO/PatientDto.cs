namespace HospitalManagement_BvDKAnViet.Core.DTOs.PatientDTO
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
    }
}
