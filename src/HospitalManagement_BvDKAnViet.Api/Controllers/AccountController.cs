using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;
using HospitalManagement_BvDKAnViet.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountController(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        // GET: api/Account
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _accountRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(dtos);
        }

        // GET: api/Account/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if (user is null) return NotFound();
            return Ok(_mapper.Map<UserDto>(user));
        }

        // POST: api/Account
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // repository also checks uniqueness; return Conflict for clarity
            if (await _accountRepository.IsUsernameTakenAsync(model.Username))
                return Conflict(new { message = "Username already taken" });

            var created = await _accountRepository.CreateAsync(model);
            if (created is null)
                return BadRequest(new { message = "Unable to create user" });

            var dto = _mapper.Map<UserDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = dto.UserId }, dto);
        }

        // PUT: api/Account/5
        // NOTE: Username is an immutable public identifier — controller will NOT allow changing it.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.UserId != id) return BadRequest(new { message = "Id mismatch" });

            // Do not accept or process Username changes here (immutable identifier).
            // The repository UpdateAsync only updates role and optional password when provided.
            var ok = await _accountRepository.UpdateAsync(model);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE: api/Account/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _accountRepository.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // POST: api/Account/ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = await _accountRepository.ChangePasswordAsync(model);
            if (!ok) return BadRequest(new { message = "Current password invalid or user not found" });
            return NoContent();
        }
    }
}