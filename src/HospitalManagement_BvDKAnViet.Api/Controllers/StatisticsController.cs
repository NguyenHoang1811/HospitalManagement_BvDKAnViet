using HospitalManagement_BvDKAnViet.Core.Enums;
using HospitalManagement_BvDKAnViet.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;

        public StatisticsController(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        // ─────────────────────────────────────────────
        // 1. Số lịch theo ngày / tháng / năm
        // GET: api/Statistics/appointments?type=day&value=2026-04-30
        // GET: api/Statistics/appointments?type=month&value=2026-04
        // GET: api/Statistics/appointments?type=year&value=2026
        // ─────────────────────────────────────────────
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentCount(
            [FromQuery] string type = "month",
            [FromQuery] string value = "")
        {
            var all = await _appointmentRepository.GetAllAsync();

            IEnumerable<object> grouped;

            switch (type.ToLower())
            {
                case "day":
                    // value = "2026-04" → lịch theo từng ngày trong tháng đó
                    if (!DateOnly.TryParse(value + "-01", out var monthStart))
                        monthStart = DateOnly.FromDateTime(DateTime.Today);

                    grouped = all
                        .Where(a => a.AppointmentDate.Year == monthStart.Year
                                 && a.AppointmentDate.Month == monthStart.Month)
                        .GroupBy(a => a.AppointmentDate)
                        .OrderBy(g => g.Key)
                        .Select(g => new
                        {
                            label = g.Key.ToString("dd/MM"),
                            total = g.Count(),
                            cancelled = g.Count(a => a.Status == (int)AppointmentStatus.CANCELLED),
                            completed = g.Count(a => a.Status == (int)AppointmentStatus.COMPLETED),
                            pending = g.Count(a => a.Status == (int)AppointmentStatus.PENDING),
                            confirmed = g.Count(a => a.Status == (int)AppointmentStatus.CONFIRMED)
                        });
                    break;

                case "month":
                    // value = "2026" → lịch theo từng tháng trong năm đó
                    if (!int.TryParse(value, out var year))
                        year = DateTime.Today.Year;

                    grouped = all
                        .Where(a => a.AppointmentDate.Year == year)
                        .GroupBy(a => a.AppointmentDate.Month)
                        .OrderBy(g => g.Key)
                        .Select(g => new
                        {
                            label = $"Tháng {g.Key}",
                            total = g.Count(),
                            cancelled = g.Count(a => a.Status == (int)AppointmentStatus.CANCELLED),
                            completed = g.Count(a => a.Status == (int)AppointmentStatus.COMPLETED),
                            pending = g.Count(a => a.Status == (int)AppointmentStatus.PENDING),
                            confirmed = g.Count(a => a.Status == (int)AppointmentStatus.CONFIRMED)
                        });
                    break;

                case "year":
                default:
                    // Lịch theo từng năm
                    grouped = all
                        .GroupBy(a => a.AppointmentDate.Year)
                        .OrderBy(g => g.Key)
                        .Select(g => new
                        {
                            label = $"Năm {g.Key}",
                            total = g.Count(),
                            cancelled = g.Count(a => a.Status == (int)AppointmentStatus.CANCELLED),
                            completed = g.Count(a => a.Status == (int)AppointmentStatus.COMPLETED),
                            pending = g.Count(a => a.Status == (int)AppointmentStatus.PENDING),
                            confirmed = g.Count(a => a.Status == (int)AppointmentStatus.CONFIRMED)
                        });
                    break;
            }

            return Ok(grouped);
        }

        // ─────────────────────────────────────────────
        // 2. Tỷ lệ huỷ lịch
        // GET: api/Statistics/cancellation-rate?year=2026
        // ─────────────────────────────────────────────
        [HttpGet("cancellation-rate")]
        public async Task<IActionResult> GetCancellationRate([FromQuery] int? year)
        {
            var all = await _appointmentRepository.GetAllAsync();

            if (year.HasValue)
                all = all.Where(a => a.AppointmentDate.Year == year.Value);

            var total = all.Count();
            var cancelled = all.Count(a => a.Status == (int)AppointmentStatus.CANCELLED);
            var rate = total == 0 ? 0.0 : Math.Round((double)cancelled / total * 100, 1);

            return Ok(new
            {
                total,
                cancelled,
                cancellationRate = rate,
                label = $"{rate}%"
            });
        }

        // ─────────────────────────────────────────────
        // 3. Số lịch theo bác sĩ + Top bác sĩ nhiều lịch nhất
        // GET: api/Statistics/appointments-by-doctor?top=5
        // ─────────────────────────────────────────────
        [HttpGet("appointments-by-doctor")]
        public async Task<IActionResult> GetAppointmentsByDoctor([FromQuery] int top = 0)
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var doctors = await _doctorRepository.GetAllAsync();

            var query = appointments
                .GroupBy(a => a.DoctorId)
                .Select(g => new
                {
                    doctorId = g.Key,
                    doctorName = doctors.FirstOrDefault(d => d.DoctorId == g.Key)?.Name
                                 ?? $"Bác sĩ #{g.Key}",
                    total = g.Count(),
                    completed = g.Count(a => a.Status == (int)AppointmentStatus.COMPLETED),
                    cancelled = g.Count(a => a.Status == (int)AppointmentStatus.CANCELLED),
                    pending = g.Count(a => a.Status == (int)AppointmentStatus.PENDING),
                    confirmed = g.Count(a => a.Status == (int)AppointmentStatus.CONFIRMED)
                })
                .OrderByDescending(x => x.total);

            var result = top > 0 ? query.Take(top) : query;

            return Ok(result);
        }

        // ─────────────────────────────────────────────
        // 4. Số bệnh nhân mỗi bác sĩ (distinct patients)
        // GET: api/Statistics/patients-by-doctor
        // ─────────────────────────────────────────────
        [HttpGet("patients-by-doctor")]
        public async Task<IActionResult> GetPatientsByDoctor()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var doctors = await _doctorRepository.GetAllAsync();

            var result = appointments
                .GroupBy(a => a.DoctorId)
                .Select(g => new
                {
                    doctorId = g.Key,
                    doctorName = doctors.FirstOrDefault(d => d.DoctorId == g.Key)?.Name
                                    ?? $"Bác sĩ #{g.Key}",
                    patientCount = g.Select(a => a.PatientId).Distinct().Count()
                })
                .OrderByDescending(x => x.patientCount);

            return Ok(result);
        }

        // ─────────────────────────────────────────────
        // 5. Tổng số bệnh nhân
        // GET: api/Statistics/total-patients
        // ─────────────────────────────────────────────
        [HttpGet("total-patients")]
        public async Task<IActionResult> GetTotalPatients()
        {
            var patients = await _patientRepository.GetAllAsync();

            return Ok(new
            {
                total = patients.Count()
            });
        }

        // ─────────────────────────────────────────────
        // 6. Bệnh nhân mới theo tháng
        // GET: api/Statistics/new-patients?year=2026
        // ─────────────────────────────────────────────
        [HttpGet("new-patients")]
        public async Task<IActionResult> GetNewPatientsByMonth([FromQuery] int? year)
        {
            var patients = await _patientRepository.GetAllAsync();

            int targetYear = year ?? DateTime.Today.Year;

            // Dùng lần đầu xuất hiện trong bảng Appointment làm "ngày tạo"
            var appointments = await _appointmentRepository.GetAllAsync();

            var firstVisit = appointments
                .GroupBy(a => a.PatientId)
                .Select(g => new
                {
                    patientId = g.Key,
                    firstDate = g.Min(a => a.AppointmentDate)
                });

            var result = firstVisit
                .Where(x => x.firstDate.Year == targetYear)
                .GroupBy(x => x.firstDate.Month)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    label = $"Tháng {g.Key}",
                    month = g.Key,
                    newPatients = g.Count()
                });

            return Ok(result);
        }

        // ─────────────────────────────────────────────
        // 7. Bệnh nhân quay lại (có >= 2 lịch hẹn)
        // GET: api/Statistics/returning-patients
        // ─────────────────────────────────────────────
        [HttpGet("returning-patients")]
        public async Task<IActionResult> GetReturningPatients()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var patients = await _patientRepository.GetAllAsync();

            var grouped = appointments
                .GroupBy(a => a.PatientId)
                .Select(g => new
                {
                    patientId = g.Key,
                    patientName = patients.FirstOrDefault(p => p.PatientId == g.Key)?.Name
                                      ?? $"BN #{g.Key}",
                    appointmentCount = g.Count(),
                    isReturning = g.Count() >= 2
                });

            var returning = grouped.Where(x => x.isReturning)
                                   .OrderByDescending(x => x.appointmentCount);

            return Ok(new
            {
                totalReturning = returning.Count(),
                totalPatients = grouped.Count(),
                returningRate = grouped.Count() == 0 ? 0
                    : Math.Round((double)returning.Count() / grouped.Count() * 100, 1),
                patients = returning
            });
        }

        // ─────────────────────────────────────────────
        // 8. Dashboard tổng hợp (gọi 1 lần lấy tất cả)
        // GET: api/Statistics/dashboard
        // ─────────────────────────────────────────────
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var patients = await _patientRepository.GetAllAsync();
            var doctors = await _doctorRepository.GetAllAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var thisMonth = DateTime.Today.Month;
            var thisYear = DateTime.Today.Year;

            var totalAppts = appointments.Count();
            var todayAppts = appointments.Count(a => a.AppointmentDate == today);
            var monthAppts = appointments.Count(a => a.AppointmentDate.Year == thisYear
                                                      && a.AppointmentDate.Month == thisMonth);
            var cancelledAppts = appointments.Count(a => a.Status == (int)AppointmentStatus.CANCELLED);
            var pendingAppts = appointments.Count(a => a.Status == (int)AppointmentStatus.PENDING);

            var cancellationRate = totalAppts == 0 ? 0
                : Math.Round((double)cancelledAppts / totalAppts * 100, 1);

            var returningCount = appointments
                .GroupBy(a => a.PatientId)
                .Count(g => g.Count() >= 2);

            return Ok(new
            {
                // KPI cards
                totalPatients = patients.Count(),
                totalDoctors = doctors.Count(),
                totalAppointments = totalAppts,
                todayAppointments = todayAppts,
                monthAppointments = monthAppts,
                pendingAppointments = pendingAppts,
                cancelledAppointments = cancelledAppts,
                cancellationRate = cancellationRate,
                returningPatients = returningCount,

                // Top 5 bác sĩ
                topDoctors = appointments
                    .GroupBy(a => a.DoctorId)
                    .Select(g => new
                    {
                        doctorId = g.Key,
                        doctorName = doctors.FirstOrDefault(d => d.DoctorId == g.Key)?.Name
                                     ?? $"#{g.Key}",
                        total = g.Count()
                    })
                    .OrderByDescending(x => x.total)
                    .Take(5)
            });
        }
    }
}