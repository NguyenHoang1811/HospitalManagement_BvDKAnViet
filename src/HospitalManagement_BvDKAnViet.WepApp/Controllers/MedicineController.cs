using HospitalManagement_BvDKAnViet.WepApp.Models.MedicineDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MedicineController : Controller
    {
        private readonly IApiService _apiService;

        public MedicineController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Medicine
        public async Task<IActionResult> Index()
        {
            try
            {
                var items = await _apiService.GetAsync<IEnumerable<MedicineDto>>("api/Medicine");
                return View(items ?? Enumerable.Empty<MedicineDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(Enumerable.Empty<MedicineDto>());
            }
        }

        // GET: /Medicine/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateMedicineDto());
        }

        // POST: /Medicine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMedicineDto model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var created = await _apiService.PostAsync<CreateMedicineDto, MedicineDto>("api/Medicine", model);
                if (created is null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo thuốc");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Thuốc được tạo thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /Medicine/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var item = await _apiService.GetAsync<MedicineDto>($"api/Medicine/{id}");
                if (item is null) return NotFound();

                var updateDto = new UpdateMedicineDto
                {
                    Name = item.Name,
                    Price = item.Price
                };

                ViewBag.MedicineId = id;
                return View(updateDto);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Medicine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateMedicineDto model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                await _apiService.PutAsync($"api/Medicine/{id}", model);
                TempData["SuccessMessage"] = "Cập nhật thuốc thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        // GET: /Medicine/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _apiService.GetAsync<MedicineDto>($"api/Medicine/{id}");
                if (item is null) return NotFound();
                return View(item);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Medicine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Medicine/{id}");
                TempData["SuccessMessage"] = "Xóa thuốc thành công";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => "Không thể xóa thuốc",
                    System.Net.HttpStatusCode.NotFound => "Thuốc không tồn tại",
                    _ => "Đã xảy ra lỗi, vui lòng thử lại sau"
                };
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
