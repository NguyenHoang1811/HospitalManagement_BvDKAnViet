using HospitalManagement_BvDKAnViet.WepApp.Model.MedicineDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.PrescriptionDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IApiService _apiService;

        public PrescriptionController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Prescription/ByRecord/5
        // GET: /Prescription/ByRecord/5
        public async Task<IActionResult> ByRecord(int recordId)
        {
            try
            {
                // Lấy tất cả rồi filter theo recordId
                var all = await _apiService
                    .GetAsync<IEnumerable<PrescriptionDto>>("api/Prescription");

                var prescriptions = all?
                    .Where(p => p.RecordId == recordId)
                    ?? Enumerable.Empty<PrescriptionDto>();

                ViewBag.RecordId = recordId;
                await LoadMedicineDropdown();
                return View(prescriptions);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction("Details", "MedicalRecord", new { id = recordId });
            }
        }

        // POST: /Prescription/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePrescriptionDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction(nameof(ByRecord), new { recordId = model.RecordId });
            }

            try
            {
                await _apiService.PostAsync<CreatePrescriptionDto, PrescriptionDto>(
                    "api/Prescription", model);
                TempData["SuccessMessage"] = "Thêm đơn thuốc thành công";
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
            }

            return RedirectToAction(nameof(ByRecord), new { recordId = model.RecordId });
        }

        // POST: /Prescription/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePrescriptionDto model)
        {
            try
            {
                await _apiService.PutAsync($"api/Prescription/{id}", model);
                TempData["SuccessMessage"] = "Cập nhật đơn thuốc thành công";
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
            }

            return RedirectToAction(nameof(ByRecord), new { recordId = model.RecordId });
        }

        // POST: /Prescription/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int recordId)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Prescription/{id}");
                TempData["SuccessMessage"] = "Xóa đơn thuốc thành công";
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
            }

            return RedirectToAction(nameof(ByRecord), new { recordId });
        }

        private async Task LoadMedicineDropdown()
        {
            try
            {
                var medicines = await _apiService
                    .GetAsync<IEnumerable<MedicineDto>>("api/Medicine");
                ViewBag.Medicines = new SelectList(
                    medicines ?? Enumerable.Empty<MedicineDto>(),
                    "MedicineId", "Name");
            }
            catch
            {
                ViewBag.Medicines = new SelectList(Enumerable.Empty<object>());
            }
        }
    }
}