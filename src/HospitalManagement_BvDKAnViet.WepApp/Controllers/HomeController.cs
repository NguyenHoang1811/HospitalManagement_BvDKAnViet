using HospitalManagement_BvDKAnViet.WepApp.Models;
using HospitalManagement_BvDKAnViet.WepApp.Models.StatisticsDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize(Roles = "Admin,Staff,Doctor")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApiService _apiService;

        public HomeController(ILogger<HomeController> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        // GET: /Home/Index — Dashboard tổng hợp
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = await _apiService
                    .GetAsync<DashboardDto>("api/Statistics/dashboard");

                return View(dashboard ?? new DashboardDto());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(new DashboardDto());
            }
        }

        // GET: /Home/AppointmentStats?type=month&value=2026
        [HttpGet]
        public async Task<IActionResult> AppointmentStats(
            string type = "month",
            string value = "")
        {
            if (string.IsNullOrEmpty(value))
                value = type switch
                {
                    "day" => DateTime.Today.ToString("yyyy-MM"),
                    "month" => DateTime.Today.Year.ToString(),
                    _ => DateTime.Today.Year.ToString()
                };

            try
            {
                var data = await _apiService
                    .GetAsync<IEnumerable<AppointmentStatDto>>(
                        $"api/Statistics/appointments?type={type}&value={value}");

                return Json(data ?? Enumerable.Empty<AppointmentStatDto>());
            }
            catch
            {
                return Json(Enumerable.Empty<AppointmentStatDto>());
            }
        }

        // GET: /Home/CancellationRate?year=2026
        [HttpGet]
        public async Task<IActionResult> CancellationRate(int? year)
        {
            var query = year.HasValue ? $"?year={year}" : "";

            try
            {
                var data = await _apiService
                    .GetAsync<CancellationRateDto>(
                        $"api/Statistics/cancellation-rate{query}");

                return Json(data ?? new CancellationRateDto());
            }
            catch
            {
                return Json(new CancellationRateDto());
            }
        }

        // GET: /Home/AppointmentsByDoctor?top=5
        [HttpGet]
        public async Task<IActionResult> AppointmentsByDoctor(int top = 0)
        {
            var query = top > 0 ? $"?top={top}" : "";

            try
            {
                var data = await _apiService
                    .GetAsync<IEnumerable<DoctorAppointmentStatDto>>(
                        $"api/Statistics/appointments-by-doctor{query}");

                return Json(data ?? Enumerable.Empty<DoctorAppointmentStatDto>());
            }
            catch
            {
                return Json(Enumerable.Empty<DoctorAppointmentStatDto>());
            }
        }

        // GET: /Home/PatientsByDoctor
        [HttpGet]
        public async Task<IActionResult> PatientsByDoctor()
        {
            try
            {
                var data = await _apiService
                    .GetAsync<IEnumerable<DoctorPatientStatDto>>(
                        "api/Statistics/patients-by-doctor");

                return Json(data ?? Enumerable.Empty<DoctorPatientStatDto>());
            }
            catch
            {
                return Json(Enumerable.Empty<DoctorPatientStatDto>());
            }
        }

        // GET: /Home/NewPatients?year=2026
        [HttpGet]
        public async Task<IActionResult> NewPatients(int? year)
        {
            var query = year.HasValue ? $"?year={year}" : "";

            try
            {
                var data = await _apiService
                    .GetAsync<IEnumerable<NewPatientStatDto>>(
                        $"api/Statistics/new-patients{query}");

                return Json(data ?? Enumerable.Empty<NewPatientStatDto>());
            }
            catch
            {
                return Json(Enumerable.Empty<NewPatientStatDto>());
            }
        }

        // GET: /Home/ReturningPatients
        [HttpGet]
        public async Task<IActionResult> ReturningPatients()
        {
            try
            {
                var data = await _apiService
                    .GetAsync<ReturningPatientDto>(
                        "api/Statistics/returning-patients");

                return Json(data ?? new ReturningPatientDto());
            }
            catch
            {
                return Json(new ReturningPatientDto());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}