using HospitalManagement_BvDKAnViet.WepApp.Models.DoctorDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.PatientDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.PrescriptionDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

                // ── Load thêm thông tin bệnh nhân để hiển thị sidebar ──
                try
                {
                    var patient = await _apiService.GetAsync<PatientDto>($"api/Patient/{record.PatientId}");
                    if (patient is not null)
                    {
                        ViewBag.PatientAddress = patient.Address;
                        ViewBag.PatientDOB = patient.DateOfBirth;   
                        ViewBag.PatientPhone = patient.Phone;
                    }
                }
                catch
                {
                    // Không bắt buộc — sidebar sẽ hiển thị "—" nếu thiếu data
                }

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
        public async Task<IActionResult> Create(int? patientId)
        {
            await LoadDropdownData();
            var model = new CreateMedicalRecordDto();
            if (patientId.HasValue)
            {
                model.PatientId = patientId.Value;
                try
                {
                    var patient = await _apiService.GetAsync<PatientDto>($"api/Patient/{patientId}");
                    ViewBag.PatientName = patient?.Name;
                }
                catch { }
            }
            return View(model);
        }

        // POST: /MedicalRecord/Create
        // POST: /MedicalRecord/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMedicalRecordDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(model);
            }

            try
            {
                // ===== TẠO RECORD =====
                var created = await _apiService.PostAsync<CreateMedicalRecordDto, MedicalRecordDto>(
                    "api/MedicalRecord", model);

                if (created is null)
                {
                    ModelState.AddModelError("", "Không thể tạo hồ sơ y tế");
                    await LoadDropdownData();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Hồ sơ y tế được tạo thành công";
                return RedirectToAction("Details", new { id = created.RecordId });
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest =>
                        "Bệnh nhân đang điều trị, không thể tạo thêm bệnh án",
                    System.Net.HttpStatusCode.NotFound =>
                        "Không tìm thấy bệnh nhân hoặc bác sĩ",
                    _ => "Không thể kết nối tới máy chủ"
                };

                await LoadDropdownData();
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

                
                if (record.MedicalRecordStatus == "XuatVien")
                {
                    TempData["ErrorMessage"] = "Không thể sửa bệnh án đã xuất viện";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.RecordId = id;

                var updateDto = new UpdateMedicalRecordDto
                {
                    PatientId = record.PatientId,
                    DoctorId = record.DoctorId,
                    Symptoms = record.Symptoms,
                    Diagnosis = record.Diagnosis,
                    Treatment = record.Treatment,
                    MedicalRecordStatus = record.MedicalRecordStatus,
                    Attachment = record.Attachment,
                    Result = record.Result,
                    Note = record.Note
                };

                await LoadDropdownData();
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
        public async Task<IActionResult> Edit(int id, UpdateMedicalRecordDto model, IFormFile? file, bool RemoveAttachment)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(model);
            }

            try
            {
                // ===== XOÁ FILE =====
                if (RemoveAttachment && !string.IsNullOrEmpty(model.Attachment))
                {
                    var oldPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        model.Attachment.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath); 
                    }

                    model.Attachment = null; 
                }

                // ===== Nếu có upload file mới =====
                if (file != null && file.Length > 0)
                {
                    // xoá file cũ nếu có
                    if (!string.IsNullOrEmpty(model.Attachment))
                    {
                        var oldPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            model.Attachment.TrimStart('/')
                        );

                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    var folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/img/medicalrecord",
                        id.ToString()
                    );

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    model.Attachment = $"/img/medicalrecord/{id}/{fileName}";
                }
                // nếu không upload → giữ nguyên Attachment (nhờ hidden input)

                await _apiService.PutAsync($"api/MedicalRecord/{id}", model);

                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Không thể kết nối tới máy chủ");
                await LoadDropdownData();
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
                // ===== LẤY RECORD TRƯỚC =====
                var record = await _apiService.GetAsync<MedicalRecordDto>($"api/MedicalRecord/{id}");
                // ===== XÓA FILE ĐÍNH KÈM =====
                if (record != null && !string.IsNullOrEmpty(record.Attachment))
                {
                    var folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/img/medicalrecord",
                        id.ToString()
                    );

                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true); // xóa toàn bộ folder
                    }
                }

                // ===== XÓA ĐƠN THUỐC THEO BỆNH ÁN =====
                try
                {
                    var prescriptions = await _apiService
                        .GetAsync<IEnumerable<PrescriptionDto>>("api/Prescription");

                    var toDelete = prescriptions?
                        .Where(p => p.RecordId == id)
                        ?? Enumerable.Empty<PrescriptionDto>();

                    foreach (var p in toDelete)
                    {
                        await _apiService.DeleteAsync($"api/Prescription/{p.PrescriptionId}");
                    }
                }
                catch
                {
                    // Nếu không xóa được đơn thuốc thì vẫn tiếp tục xóa bệnh án
                }

                // ===== XÓA DB =====
                await _apiService.DeleteAsync($"api/MedicalRecord/{id}");

                TempData["SuccessMessage"] = "Xóa hồ sơ thành công";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => "Không thể xoá bệnh án đã xuất viện",
                    System.Net.HttpStatusCode.NotFound => "Không tìm thấy bệnh án",
                    _ => "Đã xảy ra lỗi, vui lòng thử lại sau"
                };
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /MedicalRecord/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<IActionResult> ByPatient(int patientId)
        {
            try
            {
                var records = await _apiService.GetAsync<IEnumerable<MedicalRecordDto>>(
                    $"api/MedicalRecord/patient/{patientId}");
                return View("Index", records ?? Enumerable.Empty<MedicalRecordDto>());
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
                return View("Index", records ?? Enumerable.Empty<MedicalRecordDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                var patients = await _apiService.GetAsync<IEnumerable<PatientDto>>("api/Patient");
                ViewBag.Patients = new SelectList(
                    patients ?? Enumerable.Empty<PatientDto>(),
                    "PatientId", "Name");
            }
            catch { ViewBag.Patients = new SelectList(Enumerable.Empty<object>()); }

            try
            {
                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");
                ViewBag.Doctors = new SelectList(
                    doctors ?? Enumerable.Empty<DoctorDto>(),
                    "DoctorId", "Name");
            }
            catch { ViewBag.Doctors = new SelectList(Enumerable.Empty<object>()); }
        }
    }
}