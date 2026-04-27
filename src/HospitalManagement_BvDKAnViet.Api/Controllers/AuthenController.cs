using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;
using HospitalManagement_BvDKAnViet.Core.Enums;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthenController(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestData)
        {
            try
            {
                if (requestData == null
                    || string.IsNullOrWhiteSpace(requestData.Username)
                    || string.IsNullOrWhiteSpace(requestData.Password))
                {
                    return Ok(new
                    {
                        ResponseCode = (int)AccountManagerStatus.ACCOUNT_NAME_NOT_VALID,
                        ResponseMessage = "Tên hoặc mật khẩu đăng nhập không đúng",
                        token = string.Empty,
                        UserName = string.Empty,
                        AccountID = 0,
                        resfeshToken = string.Empty
                    });
                }

                var account = await _accountRepository.Account_Login(requestData);
                if (account == null)
                {
                    return Ok(new
                    {
                        ResponseCode = (int)AccountManagerStatus.ACCOUNT_NAME_NOT_VALID,
                        ResponseMessage = "Tên hoặc mật khẩu đăng nhập không đúng",
                        token = string.Empty,
                        UserName = string.Empty,
                        AccountID = 0,
                        resfeshToken = string.Empty
                    });
                }

                // Build claims
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.Username),
                    new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString())
                };

                if (account.Role != null && !string.IsNullOrWhiteSpace(account.Role.RoleName))
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, account.Role.RoleName));
                }

                // Generate JWT (uses registered IJwtService)
                var token = _jwtService.GenerateToken(account);

                // Generate refresh token
                var refreshToken = GenerateRefreshToken();

                // Persist refresh token and expiry using IJwtService
                // read expiry days from configuration (default 7 days)
                var refreshExpiryDays = _configuration.GetValue<int?>("Jwt:RefreshTokenExpiresInDays") ?? 7;
                var refreshExpiry = DateTime.UtcNow.AddDays(refreshExpiryDays);

                // update account entity and persist
                account.RefreshToken = refreshToken;
                account.ExpriredTime = refreshExpiry;

                // Persist via IJwtService implementation
                await _jwtService.UpdateRefreshToken(account);

                return Ok(new
                {
                    ResponseCode = (int)AccountManagerStatus.ACCOUNT_INSERT_SUCCESS,
                    ResponseMessage = "Đăng nhập thành công",
                    token = token,
                    UserName = account.Username,
                    AccountID = account.UserId,
                    resfeshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private static string GenerateRefreshToken(int size = 64)
        {
            var randomBytes = new byte[size];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        //[HttpGet("generate-hash")]
        //public IActionResult GenerateHash(string password)
        //{
        //    var hash = AccountRepository.CreatePasswordHash(password);
        //    return Ok(hash);
        //}
    }
}
