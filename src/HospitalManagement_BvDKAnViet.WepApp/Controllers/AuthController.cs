using HospitalManagement_BvDKAnViet.WepApp.Models.AccountDTO;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IApiService apiService, ITokenProvider tokenProvider)
        {
            _apiService = apiService;
            _tokenProvider = tokenProvider;
        }

        // Allow anonymous so users can reach the login page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var token = _tokenProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Allow anonymous POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _apiService.PostAsync<LoginRequestDto, LoginApiResponse>(
                    "api/Authen/Login",
                    model
                );

                if (response?.ResponseCode == 1 && !string.IsNullOrWhiteSpace(response.token))
                {
                    // Persist token via token provider (used by ApiService's AuthTokenHandler)
                    _tokenProvider.SetToken(response.token);

                    // Store additional user info in session
                    HttpContext.Session.SetString("Username", response.UserName ?? string.Empty);
                    HttpContext.Session.SetInt32("AccountID", response.AccountID);

                    // Create authentication cookie so ASP.NET Core considers the user authenticated
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, response.UserName ?? string.Empty),
                            new Claim("AccessToken", response.token)
                        };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, response?.ResponseMessage ?? "Đăng nhập thất bại");
                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Clear all session data and token provider
            HttpContext.Session.Clear();
            _tokenProvider.RemoveToken();

            // Sign out cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Internal DTO matching API login response structure
        private class LoginApiResponse
        {
            public int ResponseCode { get; set; }
            public string ResponseMessage { get; set; } = string.Empty;
            public string token { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public int AccountID { get; set; }
            public string resfeshToken { get; set; } = string.Empty;
        }
    }
}
