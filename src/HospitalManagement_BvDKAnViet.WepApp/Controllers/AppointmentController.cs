using HospitalManagement_BvDKAnViet.WepApp.Models.AppointmentDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.PatientDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.DoctorDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IApiService _apiService;

        public AppointmentController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Appointment
        public async Task<IActionResult> Index()
        {
            try
            {
                string url;

                if (User.IsInRole("Doctor"))
                {
                    var doctorId = User.FindFirstValue("DoctorId") ?? "0";
                    url = $"api/Appointment/doctor/{doctorId}";
                }
                else
                {
                    url = "api/Appointment";
                }

                var appointments = await _apiService.GetAsync<IEnumerable<AppointmentDto>>(url);

                // 🔥 Lấy danh sách bệnh nhân + bác sĩ
                var patients = await _apiService.GetAsync<IEnumerable<PatientDto>>("api/Patient");
                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");

                // 🔥 Tạo Dictionary để tra cứu nhanh
                ViewBag.PatientNames = patients?
                    .ToDictionary(p => p.PatientId, p => p.Name);

                ViewBag.DoctorNames = doctors?
                    .ToDictionary(d => d.DoctorId, d => d.Name);

                return View(appointments ?? Enumerable.Empty<AppointmentDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(Enumerable.Empty<AppointmentDto>());
            }
        }

        // GET: /Appointment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var item = await _apiService.GetAsync<AppointmentDto>($"api/Appointment/{id}");
                if (item is null) return NotFound();

                // Fetch tên bệnh nhân và bác sĩ song song
                var patientTask = _apiService.GetAsync<PatientDto>($"api/Patient/{item.PatientId}");
                var doctorTask = _apiService.GetAsync<DoctorDto>($"api/Doctor/{item.DoctorId}");
                await Task.WhenAll(patientTask, doctorTask);

                ViewBag.PatientName = patientTask.Result?.Name ?? item.PatientId.ToString();
                ViewBag.DoctorName = doctorTask.Result?.Name ?? item.DoctorId.ToString();

                return View(item);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Appointment/Create

        // GET: /Appointment/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateAppointmentDto
            {
                // Mặc định ngày hẹn là ngày mai
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            // ✅ Patient không cần chọn bệnh nhân — tự lấy từ session
            if (User.IsInRole("Patient"))
            {
                var patientId = HttpContext.Session.GetInt32("PatientId") ?? 0;
                model.PatientId = patientId;
                ViewBag.IsPatient = true;
            }
            else
            {
                ViewBag.IsPatient = false;
                await LoadPatients();
            }

            await LoadDoctors();
            return View(model);
        }

        // POST: /Appointment/Create
        // POST: /Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentDto model)
        {
            if (User.IsInRole("Patient"))
                ModelState.Remove("PatientId");

            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(model);
            }

            try
            {
                var created = await _apiService
                    .PostAsync<CreateAppointmentDto, AppointmentDto>("api/Appointment", model);

                if (created is null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo lịch hẹn");
                    await LoadDropdownData();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Đặt lịch thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                // ✅ Lấy message trực tiếp từ API response
                var errorMessage = ex.Message switch
                {
                    var m when m.Contains("quá khứ") => "Không được đặt lịch trong quá khứ",
                    var m when m.Contains("thứ 2") => "Chỉ đặt lịch từ thứ 2 đến thứ 6",
                    var m when m.Contains("hành chính") => "Chỉ đặt lịch trong giờ hành chính (08:00 - 17:00)",
                    var m when m.Contains("nghỉ trưa") => "Không thể đặt lịch trong giờ nghỉ trưa (12:00 - 13:00)",
                    var m when m.Contains("15 phút") => "Chỉ được đặt lịch theo mỗi 15 phút (08:00, 08:15, 08:30...)",
                    var m when m.Contains("Doctor is not available") => "Bác sĩ không có lịch trống tại thời điểm này",
                    var m when m.Contains("Patient already") => "Bệnh nhân đã có lịch hẹn tại thời điểm này",
                    var m when m.Contains("Patient or Doctor not found") => "Không tìm thấy bệnh nhân hoặc bác sĩ",
                    _ => ex.Message // ✅ Hiện nguyên message nếu không khớp pattern nào
                };

                ModelState.AddModelError(string.Empty, errorMessage);
                await LoadDropdownData();
                return View(model);
            }
        }

        // GET: /Appointment/Edit/5
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var item = await _apiService.GetAsync<AppointmentDto>($"api/Appointment/{id}");
                if (item is null) return NotFound();

                var updateDto = new UpdateAppointmentDto
                {
                    AppointmentId = item.AppointmentId,
                    PatientId = item.PatientId,
                    DoctorId = item.DoctorId,
                    AppointmentDate = item.AppointmentDate,
                    AppointmentTime = item.AppointmentTime,
                    Status = item.Status
                };

                ViewBag.AppointmentId = id;
                await LoadDropdownData();
                return View(updateDto);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Appointment/Edit/5
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAppointmentDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(model);
            }

            if (id != model.AppointmentId) return BadRequest();

            try
            {
                await _apiService.PutAsync($"api/Appointment/{id}", model);
                TempData["SuccessMessage"] = "Cập nhật lịch hẹn thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                // ✅ Tương tự Create — map lỗi rõ ràng
                var errorMessage = ex.Message switch
                {
                    var m when m.Contains("quá khứ") => "Không được đặt lịch trong quá khứ",
                    var m when m.Contains("thứ 2") => "Chỉ đặt lịch từ thứ 2 đến thứ 6",
                    var m when m.Contains("hành chính") => "Chỉ đặt lịch trong giờ hành chính (08:00 - 17:00)",
                    var m when m.Contains("nghỉ trưa") => "Không thể đặt lịch trong giờ nghỉ trưa (12:00 - 13:00)",
                    var m when m.Contains("15 phút") => "Chỉ được đặt lịch theo mỗi 15 phút (08:00, 08:15, 08:30...)",
                    var m when m.Contains("Doctor is not available") => "Bác sĩ không có lịch trống tại thời điểm này",
                    var m when m.Contains("Patient already") => "Bệnh nhân đã có lịch hẹn tại thời điểm này",
                    var m when m.Contains("Patient or Doctor not found") => "Không tìm thấy bệnh nhân hoặc bác sĩ",
                    _ => ex.Message
                };

                ModelState.AddModelError(string.Empty, errorMessage);
                await LoadDropdownData();
                return View(model);
            }
        }

        // POST: /Appointment/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Appointment/{id}");
                TempData["SuccessMessage"] = "Xóa lịch hẹn thành công";
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể xóa lịch hẹn";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownData()
        {
            await LoadPatients();
            await LoadDoctors();
        }

        private async Task LoadPatients()
        {
            try
            {
                var patients = await _apiService.GetAsync<IEnumerable<PatientDto>>("api/Patient");
                ViewBag.Patients = new SelectList(
                    patients ?? Enumerable.Empty<PatientDto>(), "PatientId", "Name");
            }
            catch { ViewBag.Patients = new SelectList(Enumerable.Empty<object>()); }
        }

        private async Task LoadDoctors()
        {
            try
            {
                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");
                ViewBag.Doctors = new SelectList(
                    doctors ?? Enumerable.Empty<DoctorDto>(), "DoctorId", "Name");
            }
            catch { ViewBag.Doctors = new SelectList(Enumerable.Empty<object>()); }
        }

       
        [HttpGet("AvailableSlots")]
        public async Task<IActionResult> AvailableSlots(int doctorId, string date)
        {
            try
            {
                var slots = await _apiService.GetAsync<IEnumerable<string>>(
                    $"api/Appointment/doctor/{doctorId}/available-slots?date={date}");
                return Json(slots ?? Enumerable.Empty<string>());
            }
            catch
            {
                return Json(Enumerable.Empty<string>());
            }
        }
    }
}
