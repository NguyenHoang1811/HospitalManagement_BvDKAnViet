namespace HospitalManagement_BvDKAnViet.Core.DTOs.AppointmentDTO
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string? Status { get; set; }
    }
}
