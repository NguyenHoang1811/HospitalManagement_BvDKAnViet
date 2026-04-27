using HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.RoleDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;

        public AccountController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Account
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _apiService.GetAsync<IEnumerable<UserDto>>("api/Account");
                return View(users ?? Enumerable.Empty<UserDto>());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return View(Enumerable.Empty<UserDto>());
            }
        }

        // GET: /Account/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRoles();
            return View(new CreateUserDto());
        }

        // POST: /Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRoles();
                return View(model);
            }

            try
            {
                var created = await _apiService.PostAsync<CreateUserDto, UserDto>("api/Account", model);
                if (created is null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể tạo tài khoản (có thể trùng tên đăng nhập)");
                    await LoadRoles();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Tài khoản được tạo thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                await LoadRoles();
                return View(model);
            }
        }

        // GET: /Account/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _apiService.GetAsync<UserDto>($"api/Account/{id}");
                if (user is null) return NotFound();

                var updateDto = new UpdateUserDto
                {
                    UserId = user.UserId,
                    // Some UpdateUserDto variants include Username; set if present via reflection safe-guard
                };

                // Attempt to set Username and RoleId if properties exist on DTO
                var usernameProp = typeof(UpdateUserDto).GetProperty("Username");
                if (usernameProp != null) usernameProp.SetValue(updateDto, user.Username);

                var roleProp = typeof(UpdateUserDto).GetProperty("RoleId");
                if (roleProp != null) roleProp.SetValue(updateDto, user.RoleId);

                await LoadRoles();
                return View(updateDto);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadRoles();
                return View(model);
            }

            if (id != model.UserId) return BadRequest();

            try
            {
                await _apiService.PutAsync($"api/Account/{id}", model);
                TempData["SuccessMessage"] = "Cập nhật tài khoản thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                await LoadRoles();
                return View(model);
            }
        }

        // GET: /Account/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _apiService.GetAsync<UserDto>($"api/Account/{id}");
                if (user is null) return NotFound();
                return View(user);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Account/{id}");
                TempData["SuccessMessage"] = "Xóa tài khoản thành công";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => "Không thể xóa tài khoản",
                    System.Net.HttpStatusCode.NotFound => "Tài khoản không tồn tại",
                    _ => "Đã xảy ra lỗi, vui lòng thử lại sau"
                };
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Account/ChangePassword/5
        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
            try
            {
                var user = await _apiService.GetAsync<UserDto>($"api/Account/{id}");
                if (user is null) return NotFound();

                var model = new ChangePasswordDto { UserId = id };
                ViewBag.Username = user.Username;
                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var success = await _apiService.PostAsync<ChangePasswordDto, object>("api/Account/ChangePassword", model);
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.StatusCode == System.Net.HttpStatusCode.BadRequest
                    ? "Mật khẩu hiện tại không đúng hoặc yêu cầu không hợp lệ"
                    : "Không thể kết nối tới máy chủ");
                return View(model);
            }
        }

        private async Task LoadRoles()
        {
            try
            {
                // expected Role DTO to contain RoleId and RoleName
                var roles = await _apiService.GetAsync<IEnumerable<RoleDto>>("api/Role");
                ViewBag.Roles = new SelectList(roles ?? Enumerable.Empty<RoleDto>(), "RoleId", "RoleName");
            }
            catch
            {
                ViewBag.Roles = new SelectList(Enumerable.Empty<object>());
            }
        }
    }
}
