using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.AppointmentDTO
{
    public class CreateAppointmentDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateOnly AppointmentDate { get; set; }

        [Required]
        public string AppointmentTime { get; set; }
    }
}
