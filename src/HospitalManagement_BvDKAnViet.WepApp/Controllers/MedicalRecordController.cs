using HospitalManagement_BvDKAnViet.WepApp.Models.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly IApiService _apiService;

        public MedicalRecordController(IApiService apiService)
        {   
            _apiService = apiService;
        }

        // GET: /MedicalRecord
        public async Task<IActionResult> Index()
        {
            try
            {
                var records = await _apiService.GetAsync<IEnumerable<MedicalRecordDto>>("api/MedicalRecord");
                return View(records ?? Enumerable.Empty<MedicalRecordDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(Enumerable.Empty<MedicalRecordDto>());
            }
        }

        // GET: /MedicalRecord/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var record = await _apiService.GetAsync<MedicalRecordDto>($"api/MedicalRecord/{id}");
                if (record is null) return NotFound();
                return View(record);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /MedicalRecord/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /MedicalRecord/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMedicalRecordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var created = await _apiService.PostAsync<CreateMedicalRecordDto, MedicalRecordDto>(
                    "api/MedicalRecord", model);
                if (created is null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo hồ sơ y tế");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Hồ sơ y tế được tạo thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /MedicalRecord/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var record = await _apiService.GetAsync<MedicalRecordDto>($"api/MedicalRecord/{id}");
                if (record is null) return NotFound();

                var updateDto = new UpdateMedicalRecordDto
                {
                    PatientId = record.PatientId,
                    DoctorId = record.DoctorId,
                    Symptoms = record.Symptoms,
                    Diagnosis = record.Diagnosis,
                    Treatment = record.Treatment
                };

                return View(updateDto);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /MedicalRecord/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMedicalRecordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _apiService.PutAsync($"api/MedicalRecord/{id}", model);
                TempData["SuccessMessage"] = "Hồ sơ y tế được cập nhật thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /MedicalRecord/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var record = await _apiService.GetAsync<MedicalRecordDto>($"api/MedicalRecord/{id}");
                if (record is null) return NotFound();
                return View(record);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /MedicalRecord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/MedicalRecord/{id}");
                TempData["SuccessMessage"] = "Hồ sơ y tế được xóa thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /MedicalRecord/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<IActionResult> ByPatient(int patientId)
        {
            try
            {
                var records = await _apiService.GetAsync<IEnumerable<MedicalRecordDto>>(
                    $"api/MedicalRecord/patient/{patientId}");
                if (records is null) records = Enumerable.Empty<MedicalRecordDto>();
                return View("Index", records);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /MedicalRecord/ByDoctor/5
        [HttpGet("ByDoctor/{doctorId}")]
        public async Task<IActionResult> ByDoctor(int doctorId)
        {
            try
            {
                var records = await _apiService.GetAsync<IEnumerable<MedicalRecordDto>>(
                    $"api/MedicalRecord/doctor/{doctorId}");
                if (records is null) records = Enumerable.Empty<MedicalRecordDto>();
                return View("Index", records);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
