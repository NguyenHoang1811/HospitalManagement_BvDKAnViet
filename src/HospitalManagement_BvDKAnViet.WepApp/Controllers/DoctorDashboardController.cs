using HospitalManagement_BvDKAnViet.WepApp.Models.StatisticsDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorDashboardController : Controller
    {
        private readonly IApiService _apiService;

        public DoctorDashboardController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /DoctorDashboard
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = await _apiService
                    .GetAsync<DoctorDashboardDto>("api/DoctorStatistics/dashboard");

                return View(dashboard ?? new DoctorDashboardDto());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(new DoctorDashboardDto());
            }
        }

        // GET: /DoctorDashboard/Today — lịch hôm nay realtime
        [HttpGet]
        public async Task<IActionResult> Today()
        {
            try
            {
                var data = await _apiService
                    .GetAsync<IEnumerable<object>>("api/DoctorStatistics/today");

                return Json(data ?? Enumerable.Empty<object>());
            }
            catch
            {
                return Json(Enumerable.Empty<object>());
            }
        }

        // GET: /DoctorDashboard/ByMonth?year=2026
        [HttpGet]
        public async Task<IActionResult> ByMonth(int? year)
        {
            var query = year.HasValue ? $"?year={year}" : "";

            try
            {
                var data = await _apiService
                    .GetAsync<IEnumerable<object>>(
                        $"api/DoctorStatistics/by-month{query}");

                return Json(data ?? Enumerable.Empty<object>());
            }
            catch
            {
                return Json(Enumerable.Empty<object>());
            }
        }

        // GET: /DoctorDashboard/ByDay?year=2026&month=4
        [HttpGet]
        public async Task<IActionResult> ByDay(int? year, int? month)
        {
            var query = $"?year={year ?? DateTime.Today.Year}&month={month ?? DateTime.Today.Month}";

            try
            {
                var data = await _apiService
                    .GetAsync<IEnumerable<object>>(
                        $"api/DoctorStatistics/by-day{query}");

                return Json(data ?? Enumerable.Empty<object>());
            }
            catch
            {
                return Json(Enumerable.Empty<object>());
            }
        }

        // GET: /DoctorDashboard/Weekly
        [HttpGet]
        public async Task<IActionResult> Weekly()
        {
            try
            {
                var data = await _apiService
                    .GetAsync<object>("api/DoctorStatistics/weekly");

                return Json(data ?? new { });
            }
            catch
            {
                return Json(new { });
            }
        }

        // POST: /DoctorDashboard/Complete?id=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                await _apiService.PatchAsync($"api/Appointment/{id}/complete");
                return Json(new { success = true });
            }
            catch (HttpRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /DoctorDashboard/Cancel?id=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _apiService.PatchAsync($"api/Appointment/{id}/cancel");
                return Json(new { success = true });
            }
            catch (HttpRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}