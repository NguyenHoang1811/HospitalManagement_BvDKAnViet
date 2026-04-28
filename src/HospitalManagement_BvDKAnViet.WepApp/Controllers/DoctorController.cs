using HospitalManagement_BvDKAnViet.WepApp.Models.DepartmentDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.DoctorDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IWebHostEnvironment _env;

        public DoctorController(IApiService apiService, IWebHostEnvironment env)
        {
            _apiService = apiService;
            _env = env;
        }

        private string GetWebRoot()
        {
            return _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index()
        {
            try
            {
                var doctors = await _apiService.GetAsync<IEnumerable<DoctorDto>>("api/Doctor");
                return View(doctors ?? Enumerable.Empty<DoctorDto>());
            }
            catch
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(Enumerable.Empty<DoctorDto>());
            }
        }

        // GET: /Doctor/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var doctor = await _apiService.GetAsync<DoctorDto>($"api/Doctor/{id}");
                if (doctor == null) return NotFound();

                var departments = await _apiService.GetAsync<IEnumerable<DepartmentDto>>("api/Department");

                ViewBag.DepartmentName = departments?
                    .FirstOrDefault(x => x.DepartmentId == doctor.DepartmentId)
                    ?.DepartmentName;

                return View(doctor);
            }
            catch
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================== CREATE =====================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDepartments();
            return View(new CreateDoctorDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDoctorDto model, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(model);
            }

            try
            {
                var created = await _apiService.PostAsync<CreateDoctorDto, DoctorDto>("api/Doctor", model);

                if (created == null)
                {
                    ModelState.AddModelError("", "Không thể tạo bác sĩ");
                    await LoadDepartments();
                    return View(model);
                }

                // Upload ảnh
                if (file != null && file.Length > 0)
                {
                    var imagePath = await SaveImage(file, created.DoctorId);

                    var updateDto = new UpdateDoctorDto
                    {
                        Name = created.Name,
                        Specialty = created.Specialty,
                        Phone = created.Phone,
                        Email = created.Email,
                        DepartmentId = created.DepartmentId,
                        DoctorImage = imagePath
                    };

                    await _apiService.PutAsync($"api/Doctor/{created.DoctorId}", updateDto);
                }

                TempData["SuccessMessage"] = "Tạo bác sĩ thành công";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Không thể kết nối tới máy chủ");
                await LoadDepartments();
                return View(model);
            }
        }

        // ===================== EDIT =====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var doctor = await _apiService.GetAsync<DoctorDto>($"api/Doctor/{id}");
                if (doctor == null) return NotFound();

                var model = new UpdateDoctorDto
                {
                    Name = doctor.Name,
                    Specialty = doctor.Specialty,
                    Phone = doctor.Phone,
                    Email = doctor.Email,
                    DepartmentId = doctor.DepartmentId,
                    DoctorImage = doctor.DoctorImage
                };

                ViewBag.DoctorId = id;
                await LoadDepartments();
                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = "Không thể kết nối";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDoctorDto model, IFormFile? file, bool removeImage = false)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartments();
                return View(model);
            }

            try
            {
                var webRoot = GetWebRoot();

                // XÓA ẢNH
                if (removeImage && !string.IsNullOrEmpty(model.DoctorImage))
                {
                    var path = Path.Combine(webRoot,
                        model.DoctorImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    model.DoctorImage = null;
                }

                // UPLOAD ẢNH MỚI
                if (file != null && file.Length > 0)
                {
                    model.DoctorImage = await SaveImage(file, id);
                }

                await _apiService.PutAsync($"api/Doctor/{id}", model);

                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Không thể kết nối tới máy chủ");
                await LoadDepartments();
                return View(model);
            }
        }

        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var folder = Path.Combine(GetWebRoot(), "img", "doctor", id.ToString());

                if (Directory.Exists(folder))
                    Directory.Delete(folder, true);

                await _apiService.DeleteAsync($"api/Doctor/{id}");

                TempData["SuccessMessage"] = "Xóa thành công";
            }
            catch
            {
                TempData["ErrorMessage"] = "Không thể xóa";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== HELPER =====================
        private async Task LoadDepartments()
        {
            var data = await _apiService.GetAsync<IEnumerable<DepartmentDto>>("api/Department");

            ViewBag.Departments = new SelectList(
                data ?? Enumerable.Empty<DepartmentDto>(),
                "DepartmentId",
                "DepartmentName"
            );
        }

        private async Task<string> SaveImage(IFormFile file, int doctorId)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowed.Contains(ext))
                throw new Exception("File không hợp lệ");

            if (file.Length > 2 * 1024 * 1024)
                throw new Exception("File quá lớn");

            var folder = Path.Combine(GetWebRoot(), "img", "doctor", doctorId.ToString());

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + ext;
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/img/doctor/{doctorId}/{fileName}";
        }
    }
}