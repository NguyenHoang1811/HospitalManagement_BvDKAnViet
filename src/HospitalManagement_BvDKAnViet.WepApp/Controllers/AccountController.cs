using HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.RoleDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;

        public AccountController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Account
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create(int? patientId, int? doctorId)
        {
            await LoadRoles();

            var model = new CreateUserDto();

            // ❗ Lấy roles từ API (đúng kiểu)
            var roles = await _apiService.GetAsync<IEnumerable<RoleDto>>("api/Role");

            if (patientId != null)
            {
                model.PatientId = patientId;
                model.Username = $"BN_{patientId}";
                model.Password = "123456";
                model.ConfirmPassword = "123456";

                var role = roles?.FirstOrDefault(r => r.RoleName == "Patient");
                if (role != null) model.RoleId = role.RoleId;
            }

            if (doctorId != null)
            {
                model.DoctorId = doctorId;
                model.Username = $"BS_{doctorId}";
                model.Password = "123456";
                model.ConfirmPassword = "123456";

                var role = roles?.FirstOrDefault(r => r.RoleName == "Doctor");
                if (role != null) model.RoleId = role.RoleId;
            }

            return View(model);
        }

        // POST: /Account/Create
        [Authorize(Roles = "Admin")]
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
                    ModelState.AddModelError(string.Empty, "Không thể tạo tài khoản");
                    await LoadRoles();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Tạo tài khoản thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                string message = "Đã xảy ra lỗi";

                if (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    message = "Tên đăng nhập đã tồn tại";
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // 🔥 đọc message từ API
                    if (!string.IsNullOrEmpty(ex.Message))
                    {
                        if (ex.Message.Contains("Patient"))
                            message = "Bệnh nhân đã có tài khoản";

                        else if (ex.Message.Contains("Doctor"))
                            message = "Bác sĩ đã có tài khoản";

                        else if (ex.Message.Contains("không tồn tại"))
                            message = "Dữ liệu liên kết không tồn tại";

                        else
                            message = ex.Message;
                    }
                }
                else if (ex.StatusCode == null)
                {
                    message = "Không thể kết nối tới máy chủ";
                }

                ModelState.AddModelError(string.Empty, message);

                await LoadRoles();
                return View(model);
            }
        }

        // GET: /Account/Edit/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
            catch (HttpRequestException ex)
            {
                string message = "Đã xảy ra lỗi";

                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    message = "Dữ liệu không hợp lệ";
                }
                else if (ex.StatusCode == null)
                {
                    message = "Không thể kết nối tới máy chủ";
                }

                ModelState.AddModelError(string.Empty, message);
                await LoadRoles();
                return View(model);
            }
        }

        // GET: /Account/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                    System.Net.HttpStatusCode.BadRequest => "Không thể xóa tài khoản (có ràng buộc dữ liệu)",
                    System.Net.HttpStatusCode.NotFound => "Tài khoản không tồn tại",
                    _ => "Lỗi hệ thống, vui lòng thử lại"
                };
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Account/ChangePasswordSelf
        [HttpGet]
        public IActionResult ChangePasswordSelf()
        {
            return View(new ChangePasswordDto());
        }

        // POST: /Account/ChangePasswordSelf
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordSelf(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // ✅ Lấy UserId từ JWT đúng chuẩn
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    ModelState.AddModelError("", "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.");
                    return View(model);
                }

                model.UserId = userId;

                await _apiService.PostAsync<ChangePasswordDto, object>(
                    "api/Account/ChangePassword",
                    model
                );

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                string message = "Đã xảy ra lỗi";

                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    message = "Mật khẩu hiện tại không đúng hoặc dữ liệu không hợp lệ";
                }
                else if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    message = "Người dùng không tồn tại";
                }
                else if (ex.StatusCode == null)
                {
                    message = "Không thể kết nối tới máy chủ";
                }

                ModelState.AddModelError("", message);
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            try
            {
                await _apiService.PostAsync<object, object>($"api/Account/AdminResetPassword/{id}", null);

                TempData["SuccessMessage"] = "Đã reset mật khẩu về 123456";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "Tài khoản không tồn tại",
                    _ => "Reset mật khẩu thất bại"
                };
            }

            return RedirectToAction(nameof(Index));
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
