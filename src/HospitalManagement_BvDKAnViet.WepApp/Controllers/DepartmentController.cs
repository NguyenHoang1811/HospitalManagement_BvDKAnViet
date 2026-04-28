using HospitalManagement_BvDKAnViet.WepApp.Models.DepartmentDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IApiService _apiService;

        public DepartmentController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Department
        public async Task<IActionResult> Index()
        {
            try
            {
                var departments = await _apiService.GetAsync<IEnumerable<DepartmentDto>>("api/Department");
                return View(departments ?? Enumerable.Empty<DepartmentDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(Enumerable.Empty<DepartmentDto>());
            }
        }


        // GET: /Department/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var created = await _apiService.PostAsync<CreateDepartmentDto, DepartmentDto>("api/Department", model);
                if (created is null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo phòng ban");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Phòng ban được tạo thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /Department/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var department = await _apiService.GetAsync<DepartmentDto>($"api/Department/{id}");
                if (department is null) return NotFound();

                var updateDto = new UpdateDepartmentDto
                {
                    DepartmentName = department.DepartmentName
                };

                return View(updateDto);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDepartmentDto model)
        {
            if (!ModelState.IsValid) return View(model);
            

            try
            {
                await _apiService.PutAsync($"api/Department/{id}", model);
                TempData["SuccessMessage"] = "Cập nhật phòng ban thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /Department/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var department = await _apiService.GetAsync<DepartmentDto>($"api/Department/{id}");
                if (department is null) return NotFound();
                return View(department);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Department/{id}");
                TempData["SuccessMessage"] = "Xóa phòng ban thành công";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => "Không thể xóa phòng ban",
                    System.Net.HttpStatusCode.NotFound => "Phòng ban không tồn tại",
                    _ => "Đã xảy ra lỗi, vui lòng thử lại sau"
                };
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
