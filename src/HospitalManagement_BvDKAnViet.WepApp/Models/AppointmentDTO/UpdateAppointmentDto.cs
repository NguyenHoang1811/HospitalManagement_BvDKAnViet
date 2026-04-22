using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.AppointmentDTO
{
    public class UpdateAppointmentDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateOnly AppointmentDate { get; set; }

        [Required]
        public string AppointmentTime { get; set; }

        public string? Status { get; set; }
        
    }
}
