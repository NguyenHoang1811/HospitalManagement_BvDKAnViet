using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public DoctorController(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        // GET: api/Doctor
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return Ok(result);
        }

        // GET: api/Doctor/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor is null) return NotFound();

            var result = _mapper.Map<DoctorDto>(doctor);
            return Ok(result);
        }

        // POST: api/Doctor
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var doctor = _mapper.Map<Doctor>(dto);
            var created = await _doctorRepository.AddAsync(doctor);
            var result = _mapper.Map<DoctorDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.DoctorId }, result);
        }

        // PUT: api/Doctor/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.DoctorId) return BadRequest("Id mismatch");

            var doctor = _mapper.Map<Doctor>(dto);
            var updated = await _doctorRepository.UpdateAsync(doctor);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Doctor/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _doctorRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}