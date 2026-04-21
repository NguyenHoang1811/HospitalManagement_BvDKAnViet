using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AuthenController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        //[HttpPost("Login")]


    }
}
