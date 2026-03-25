using System;

namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public string? Status { get; set; }

        // Navigation
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}