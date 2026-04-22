using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // require authentication for all actions; role restrictions applied per action
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DoctorController(IDoctorRepository doctorRepository, IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // GET: api/Doctor
        // Admin, Staff and Doctor can read list
        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Doctor")]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return Ok(result);
        }

        // GET: api/Doctor/5
        // Admin, Staff and Doctor can read
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Staff,Doctor")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor is null) return NotFound();

            var result = _mapper.Map<DoctorDto>(doctor);
            return Ok(result);
        }

        // POST: api/Doctor
        // Admin and Staff can create (Admin has full permissions)
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // If client supplied DepartmentId, validate it exists
            if (dto.DepartmentId is not null)
            {
                var deptExists = await _departmentRepository.GetByIdAsync(dto.DepartmentId.Value) != null;
                if (!deptExists) return BadRequest("Department not found");
            }

            var doctor = _mapper.Map<Doctor>(dto);
            var created = await _doctorRepository.AddAsync(doctor);
            var result = _mapper.Map<DoctorDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.DoctorId }, result);
        }

        // PUT: api/Doctor/5
        // Admin and Staff can update (Admin has full permissions)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _doctorRepository.GetByIdAsync(id);
            if (existing is null) return NotFound();

            // If client supplied DepartmentId, validate it exists
            if (dto.DepartmentId is not null)
            {
                var deptExists = await _departmentRepository.GetByIdAsync(dto.DepartmentId.Value) != null;
                if (!deptExists) return BadRequest("Department not found");
            }

            _mapper.Map(dto, existing);

            var updated = await _doctorRepository.UpdateAsync(existing);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Doctor/5
        // Only Admin can delete (Admin full permissions)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _doctorRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}