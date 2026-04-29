using HospitalManagement_BvDKAnViet.WepApp.Models.PatientDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly IApiService _apiService;


        public PatientController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Patient
        public async Task<IActionResult> Index()
        {
            try
            {
                string url;
                if (User.IsInRole("Doctor"))
                {
                    var doctorId = User.FindFirstValue("DoctorId") ?? "0";
                    url = $"api/Patient/doctor/{doctorId}";
                }
                else
                {
                    url = "api/Patient";
                }

                var patients = await _apiService.GetAsync<IEnumerable<PatientDto>>(url);
                return View(patients ?? Enumerable.Empty<PatientDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(Enumerable.Empty<PatientDto>());
            }
        }

        // GET: /Patient/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var patient = await _apiService.GetAsync<PatientDto>($"api/Patient/{id}");
                if (patient is null) return NotFound();
                return View(patient);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Patient/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePatientDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var created = await _apiService.PostAsync<CreatePatientDto, PatientDto>("api/Patient", model);
                if (created is null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo bệnh nhân");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Bệnh nhân được tạo thành công";
                return RedirectToAction("Create", "MedicalRecord", new { patientId = created.PatientId });
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /Patient/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var patient = await _apiService.GetAsync<PatientDto>($"api/Patient/{id}");
                if (patient is null) return NotFound();

                var updateDto = new UpdatePatientDto
                {
                    Name = patient.Name,
                    DateOfBirth = patient.DateOfBirth,
                    Gender = patient.Gender,
                    Address = patient.Address,
                    Phone = patient.Phone,
                    MedicalHistory = patient.MedicalHistory
                };

                return View(updateDto);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePatientDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _apiService.PutAsync($"api/Patient/{id}", model);
                TempData["SuccessMessage"] = "Bệnh nhân được cập nhật thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /Patient/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var patient = await _apiService.GetAsync<PatientDto>($"api/Patient/{id}");
                if (patient is null) return NotFound();
                return View(patient);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Patient/{id}");
                TempData["SuccessMessage"] = "Bệnh nhân được xóa thành công";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => "Không thể xoá bệnh nhân vì đã có bệnh án",
                    System.Net.HttpStatusCode.NotFound => "Không tìm thấy bệnh nhân",
                    _ => "Đã xảy ra lỗi, vui lòng thử lại sau"
                };
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
