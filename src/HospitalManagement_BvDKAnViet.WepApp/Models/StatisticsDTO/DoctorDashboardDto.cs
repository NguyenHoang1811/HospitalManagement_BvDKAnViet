namespace HospitalManagement_BvDKAnViet.WepApp.Models.StatisticsDTO
{
    public class DoctorDashboardDto
    {
        public int Total { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public double CompletionRate { get; set; }
        public double CancellationRate { get; set; }
        public int MonthTotal { get; set; }
        public int TotalPatients { get; set; }
        public int TodayCount { get; set; }
        public int ThisWeekCount { get; set; }
        public int LastWeekCount { get; set; }
        public double WeekGrowth { get; set; }
        public IEnumerable<TodayAppointmentDto> TodayAppointments { get; set; }
            = new List<TodayAppointmentDto>();
    }

    public class TodayAppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string AppointmentTime { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
