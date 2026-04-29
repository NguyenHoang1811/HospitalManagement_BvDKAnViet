using HospitalManagement_BvDKAnViet.WepApp.Models.AppointmentDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.DepartmentDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.DoctorDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.MedicineDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.PatientDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.PrescriptionDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [AllowAnonymous]
    public class HomePublicController : Controller
    {
        private readonly IApiService _apiService;

        public HomePublicController(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<IActionResult> Index()
        {

            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Doctor") || User.IsInRole("Staff"))
                    return RedirectToAction("Index", "Home");
            }
            try
            {
                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");

                return View(doctors ?? Enumerable.Empty<DoctorDto>());
            }
            catch
            {
                return View(Enumerable.Empty<DoctorDto>());
            }
        }
        public async Task<IActionResult> Doctor()
        {
            try
            {
                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");

                return View(doctors ?? Enumerable.Empty<DoctorDto>());
            }
            catch
            {
                return View(Enumerable.Empty<DoctorDto>());
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> DoctorDetail(int id)
        {
            try
            {
                var doctor = await _apiService.GetAsync<DoctorDto>($"api/Doctor/{id}");

                if (doctor == null)
                    return NotFound();

                var departments = await _apiService.GetAsync<IEnumerable<DepartmentDto>>("api/Department");

                var departmentName = departments
                    .FirstOrDefault(d => d.DepartmentId == doctor.DepartmentId)
                    ?.DepartmentName;


                ViewBag.DepartmentName = departmentName;

                return View(doctor);

            }
            catch
            {
                return RedirectToAction(nameof(Doctor));
            }
        }


        [Authorize]
        public async Task<IActionResult> MedicalRecordByPatient()
        {
            try
            {
                var patientId = User.Claims
                    .FirstOrDefault(c => c.Type == "PatientId")?.Value;

                if (string.IsNullOrEmpty(patientId))
                    return RedirectToAction("Login", "Auth");

                var records = await _apiService.GetAsync<IEnumerable<MedicalRecordDto>>(
                    $"api/MedicalRecord/patient/{patientId}");

                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");

                // map DoctorName thủ công
                foreach (var r in records)
                {
                    var doc = doctors.FirstOrDefault(d => d.DoctorId == r.DoctorId);
                    r.DoctorName = doc?.Name;
                }

                return View(records ?? Enumerable.Empty<MedicalRecordDto>());
            }
            catch
            {
                return View(Enumerable.Empty<MedicalRecordDto>());
            }
        }

        public async Task<IActionResult> PrescriptionByRecord(int recordId)
        {
            try
            {
                var prescriptions = await _apiService.GetAsync<IEnumerable<PrescriptionDto>>(
                    $"api/Prescription/record/{recordId}");

                var medicines = await _apiService.GetAsync<IEnumerable<MedicineDto>>("api/Medicine");


                foreach (var p in prescriptions)
                {
                    var med = medicines.FirstOrDefault(m => m.MedicineId == p.MedicineId);
                    p.MedicineName = med?.Name;
                }

                ViewBag.RecordId = recordId;

                return View(prescriptions ?? Enumerable.Empty<PrescriptionDto>());
            }
            catch
            {
                return View(Enumerable.Empty<PrescriptionDto>());
            }
        }

        [Authorize]
        public async Task<IActionResult> MedicalRecordDetail(int id)
        {
            try
            {
                // 🔥 1. Lấy bệnh án
                var record = await _apiService.GetAsync<MedicalRecordDto>(
                    $"api/MedicalRecord/{id}");

                if (record == null)
                    return NotFound();

                // 🔥 2. Lấy bác sĩ
                var doctor = await _apiService.GetAsync<DoctorDto>(
                    $"api/Doctor/{record.DoctorId}");

                record.DoctorName = doctor?.Name;

                // 🔥 3. LẤY THÔNG TIN BỆNH NHÂN (BẠN ĐANG THIẾU)
                var patient = await _apiService.GetAsync<PatientDto>(
                    $"api/Patient/{record.PatientId}");

                if (patient != null)
                {
                    record.PatientName = patient.Name;

                    ViewBag.PatientAddress = patient.Address;
                    ViewBag.PatientPhone = patient.Phone;
                    ViewBag.PatientDOB = patient.DateOfBirth;
                }

                // 🔥 4. Lấy đơn thuốc
                var prescriptions = await _apiService.GetAsync<IEnumerable<PrescriptionDto>>(
                    $"api/Prescription/record/{id}");

                // 🔥 5. Map tên thuốc
                var medicines = await _apiService.GetAsync<IEnumerable<MedicineDto>>("api/Medicine");

                foreach (var p in prescriptions)
                {
                    var med = medicines.FirstOrDefault(m => m.MedicineId == p.MedicineId);
                    p.MedicineName = med?.Name;
                }

                ViewBag.Prescriptions = prescriptions;

                return View(record);
            }
            catch
            {
                return RedirectToAction(nameof(MedicalRecordByPatient));
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> BookAppointment(int doctorId)
        {
            try
            {
                var doctor = await _apiService.GetAsync<DoctorDto>($"api/Doctor/{doctorId}");
                if (doctor == null) return NotFound();

                var model = new CreateAppointmentDto
                {
                    DoctorId = doctorId,
                    AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
                };

                // 🔥 lấy patient từ claim (đúng hơn session)
                var patientId = User.Claims.FirstOrDefault(c => c.Type == "PatientId")?.Value;
                if (int.TryParse(patientId, out var pid))
                {
                    model.PatientId = pid;
                }

                ViewBag.DoctorName = doctor.Name;
                ViewBag.DoctorId = doctorId;

                return View(model);
            }
            catch
            {
                return RedirectToAction(nameof(Doctor));
            }
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(CreateAppointmentDto model)
        {
            try
            {
                var result = await _apiService
                    .PostAsync<CreateAppointmentDto, AppointmentDto>("api/Appointment", model);

                if (result == null)
                {
                    ModelState.AddModelError("", "Không thể đặt lịch");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Đặt lịch thành công!";
                return RedirectToAction(nameof(MyAppointments));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        [HttpGet]
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            try
            {
                var patientId = User.Claims.FirstOrDefault(c => c.Type == "PatientId")?.Value;

                if (string.IsNullOrEmpty(patientId))
                    return RedirectToAction("Login", "Auth");

                // Lấy tất cả lịch hẹn của bệnh nhân
                var appointments = await _apiService.GetAsync<IEnumerable<AppointmentDto>>(
                    $"api/Appointment/patient/{patientId}");

                // Lấy danh sách bác sĩ để map tên
                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");

                ViewBag.DoctorNames = doctors?.ToDictionary(d => d.DoctorId, d => d.Name)
                                      ?? new Dictionary<int, string>();

                return View(appointments ?? Enumerable.Empty<AppointmentDto>());
            }
            catch
            {
                TempData["ErrorMessage"] = "Không thể tải danh sách lịch hẹn.";
                return View(Enumerable.Empty<AppointmentDto>());
            }
        }
    }
}


