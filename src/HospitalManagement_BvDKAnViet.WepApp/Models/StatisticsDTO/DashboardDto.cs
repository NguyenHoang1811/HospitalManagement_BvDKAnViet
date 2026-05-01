namespace HospitalManagement_BvDKAnViet.WepApp.Models.StatisticsDTO
{
    // DashboardDto.cs
    public class DashboardDto
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int MonthAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public double CancellationRate { get; set; }
        public int ReturningPatients { get; set; }
        public IEnumerable<TopDoctorDto> TopDoctors { get; set; } = new List<TopDoctorDto>();
    }

    // TopDoctorDto.cs
    public class TopDoctorDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    // AppointmentStatDto.cs
    public class AppointmentStatDto
    {
        public string Label { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Cancelled { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
    }

    // CancellationRateDto.cs
    public class CancellationRateDto
    {
        public int Total { get; set; }
        public int Cancelled { get; set; }
        public double CancellationRate { get; set; }
        public string Label { get; set; } = "0%";
    }

    // DoctorAppointmentStatDto.cs
    public class DoctorAppointmentStatDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
    }

    // DoctorPatientStatDto.cs
    public class DoctorPatientStatDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int PatientCount { get; set; }
    }

    // NewPatientStatDto.cs
    public class NewPatientStatDto
    {
        public string Label { get; set; } = string.Empty;
        public int Month { get; set; }
        public int NewPatients { get; set; }
    }

    // ReturningPatientDto.cs
    public class ReturningPatientDto
    {
        public int TotalReturning { get; set; }
        public int TotalPatients { get; set; }
        public double ReturningRate { get; set; }
        public IEnumerable<PatientVisitDto> Patients { get; set; } = new List<PatientVisitDto>();
    }

    // PatientVisitDto.cs
    public class PatientVisitDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
        public bool IsReturning { get; set; }
    }
}
