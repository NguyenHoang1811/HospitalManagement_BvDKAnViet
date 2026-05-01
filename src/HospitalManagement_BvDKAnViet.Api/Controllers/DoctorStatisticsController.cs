using HospitalManagement_BvDKAnViet.Core.Enums;
using HospitalManagement_BvDKAnViet.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Authorize(Roles = "Doctor")]
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorStatisticsController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;

        public DoctorStatisticsController(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
        }

        // Lấy doctorId từ token — dùng chung cho mọi action
        private bool TryGetDoctorId(out int doctorId)
        {
            doctorId = 0;
            var claim = User.FindFirst("DoctorId")?.Value;
            return int.TryParse(claim, out doctorId) && doctorId > 0;
        }

        // ─────────────────────────────────────────────
        // 1. Dashboard tổng quan cá nhân
        // GET: api/DoctorStatistics/dashboard
        // ─────────────────────────────────────────────
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            if (!TryGetDoctorId(out var doctorId))
                return Forbid();

            var all = await _appointmentRepository.GetAllAsync();
            var mine = all.Where(a => a.DoctorId == doctorId).ToList();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var thisWeek = GetWeekRange(DateTime.Today);
            var lastWeek = GetWeekRange(DateTime.Today.AddDays(-7));
            var thisMonth = DateTime.Today.Month;
            var thisYear = DateTime.Today.Year;

            var total = mine.Count;
            var completed = mine.Count(a => a.Status == (int)AppointmentStatus.COMPLETED);
            var cancelled = mine.Count(a => a.Status == (int)AppointmentStatus.CANCELLED);
            var pending = mine.Count(a => a.Status == (int)AppointmentStatus.PENDING);
            var confirmed = mine.Count(a => a.Status == (int)AppointmentStatus.CONFIRMED);

            var todayList = mine
                .Where(a => a.AppointmentDate == today)
                .OrderBy(a => a.AppointmentTime)
                .ToList();

            var thisWeekCount = mine.Count(a =>
                a.AppointmentDate >= thisWeek.start &&
                a.AppointmentDate <= thisWeek.end);

            var lastWeekCount = mine.Count(a =>
                a.AppointmentDate >= lastWeek.start &&
                a.AppointmentDate <= lastWeek.end);

            var patients = await _patientRepository.GetAllAsync();

            return Ok(new
            {
                // KPI tổng
                total,
                completed,
                cancelled,
                pending,
                confirmed,

                // KPI hiệu suất
                completionRate = total == 0 ? 0 : Math.Round((double)completed / total * 100, 1),
                cancellationRate = total == 0 ? 0 : Math.Round((double)cancelled / total * 100, 1),

                // Tháng này
                monthTotal = mine.Count(a =>
                    a.AppointmentDate.Year == thisYear &&
                    a.AppointmentDate.Month == thisMonth),

                // Tuần này vs tuần trước
                thisWeekCount,
                lastWeekCount,
                weekGrowth = lastWeekCount == 0 ? 0
                    : Math.Round((double)(thisWeekCount - lastWeekCount) / lastWeekCount * 100, 1),

                // Hôm nay
                todayCount = todayList.Count,
                todayAppointments = todayList.Select(a => new
                {
                    a.AppointmentId,
                    a.PatientId,
                    patientName = patients.FirstOrDefault(p => p.PatientId == a.PatientId)?.Name
                                  ?? $"BN #{a.PatientId}",
                    appointmentTime = a.AppointmentTime.ToString("HH:mm"),
                    a.Status,
                    statusName = ((AppointmentStatus)a.Status).ToString()
                }),

                // Distinct patients
                totalPatients = mine.Select(a => a.PatientId).Distinct().Count()
            });
        }

        // ─────────────────────────────────────────────
        // 2. Lịch hôm nay (realtime)
        // GET: api/DoctorStatistics/today
        // ─────────────────────────────────────────────
        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            if (!TryGetDoctorId(out var doctorId))
                return Forbid();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var all = await _appointmentRepository.GetAllAsync();
            var patients = await _patientRepository.GetAllAsync();

            var list = all
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == today)
                .OrderBy(a => a.AppointmentTime)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.PatientId,
                    patientName = patients.FirstOrDefault(p => p.PatientId == a.PatientId)?.Name
                                  ?? $"BN #{a.PatientId}",
                    appointmentTime = a.AppointmentTime.ToString("HH:mm"),
                    a.Status,
                    statusName = ((AppointmentStatus)a.Status).ToString()
                });

            return Ok(list);
        }

        // ─────────────────────────────────────────────
        // 3. Biểu đồ theo tháng trong năm
        // GET: api/DoctorStatistics/by-month?year=2026
        // ─────────────────────────────────────────────
        [HttpGet("by-month")]
        public async Task<IActionResult> GetByMonth([FromQuery] int? year)
        {
            if (!TryGetDoctorId(out var doctorId))
                return Forbid();

            int targetYear = year ?? DateTime.Today.Year;
            var all = await _appointmentRepository.GetAllAsync();
            var mine = all.Where(a => a.DoctorId == doctorId
                                   && a.AppointmentDate.Year == targetYear);

            // Đảm bảo đủ 12 tháng
            var monthMap = mine
                .GroupBy(a => a.AppointmentDate.Month)
                .ToDictionary(g => g.Key, g => new
                {
                    total = g.Count(),
                    completed = g.Count(a => a.Status == (int)AppointmentStatus.COMPLETED),
                    cancelled = g.Count(a => a.Status == (int)AppointmentStatus.CANCELLED),
                    pending = g.Count(a => a.Status == (int)AppointmentStatus.PENDING)
                });

            var result = Enumerable.Range(1, 12).Select(m => new
            {
                label = $"T{m}",
                month = m,
                total = monthMap.ContainsKey(m) ? monthMap[m].total : 0,
                completed = monthMap.ContainsKey(m) ? monthMap[m].completed : 0,
                cancelled = monthMap.ContainsKey(m) ? monthMap[m].cancelled : 0,
                pending = monthMap.ContainsKey(m) ? monthMap[m].pending : 0
            });

            return Ok(result);
        }

        // ─────────────────────────────────────────────
        // 4. Biểu đồ theo ngày trong tháng (workload)
        // GET: api/DoctorStatistics/by-day?year=2026&month=4
        // ─────────────────────────────────────────────
        [HttpGet("by-day")]
        public async Task<IActionResult> GetByDay(
            [FromQuery] int? year,
            [FromQuery] int? month)
        {
            if (!TryGetDoctorId(out var doctorId))
                return Forbid();

            int targetYear = year ?? DateTime.Today.Year;
            int targetMonth = month ?? DateTime.Today.Month;

            var all = await _appointmentRepository.GetAllAsync();
            var mine = all.Where(a => a.DoctorId == doctorId
                                   && a.AppointmentDate.Year == targetYear
                                   && a.AppointmentDate.Month == targetMonth);

            var daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);

            var dayMap = mine
                .GroupBy(a => a.AppointmentDate.Day)
                .ToDictionary(g => g.Key, g => g.Count());

            var result = Enumerable.Range(1, daysInMonth).Select(d => new
            {
                label = $"{d:D2}",
                day = d,
                total = dayMap.ContainsKey(d) ? dayMap[d] : 0
            });

            return Ok(result);
        }

        // ─────────────────────────────────────────────
        // 5. Lịch tuần này vs tuần trước
        // GET: api/DoctorStatistics/weekly
        // ─────────────────────────────────────────────
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeekly()
        {
            if (!TryGetDoctorId(out var doctorId))
                return Forbid();

            var all = await _appointmentRepository.GetAllAsync();
            var mine = all.Where(a => a.DoctorId == doctorId).ToList();
            var patients = await _patientRepository.GetAllAsync();

            var thisWeek = GetWeekRange(DateTime.Today);
            var lastWeek = GetWeekRange(DateTime.Today.AddDays(-7));

            // Lịch từng ngày trong tuần này (T2 → CN)
            var thisWeekDays = Enumerable.Range(0, 7).Select(i =>
            {
                var date = thisWeek.start.AddDays(i);
                var dayAppt = mine.Where(a => a.AppointmentDate == date).ToList();
                return new
                {
                    label = date.ToString("ddd dd/MM"),
                    date = date.ToString("yyyy-MM-dd"),
                    total = dayAppt.Count,
                    completed = dayAppt.Count(a => a.Status == (int)AppointmentStatus.COMPLETED),
                    cancelled = dayAppt.Count(a => a.Status == (int)AppointmentStatus.CANCELLED)
                };
            });

            var thisWeekCount = mine.Count(a =>
                a.AppointmentDate >= thisWeek.start &&
                a.AppointmentDate <= thisWeek.end);

            var lastWeekCount = mine.Count(a =>
                a.AppointmentDate >= lastWeek.start &&
                a.AppointmentDate <= lastWeek.end);

            return Ok(new
            {
                thisWeekCount,
                lastWeekCount,
                weekGrowth = lastWeekCount == 0 ? 0
                    : Math.Round((double)(thisWeekCount - lastWeekCount) / lastWeekCount * 100, 1),
                days = thisWeekDays
            });
        }

        // Helper: lấy range tuần (Thứ 2 → Chủ nhật)
        private static (DateOnly start, DateOnly end) GetWeekRange(DateTime date)
        {
            int diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            var start = DateOnly.FromDateTime(date.AddDays(-diff));
            var end = start.AddDays(6);
            return (start, end);
        }
    }
}