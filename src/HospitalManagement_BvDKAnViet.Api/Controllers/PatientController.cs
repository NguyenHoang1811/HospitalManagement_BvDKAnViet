using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.PatientDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public PatientController(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<PatientDto>>(patients);

            return Ok(result);
        }

        // GET: api/Patient/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient is null) return NotFound();

            var result = _mapper.Map<PatientDto>(patient);
            return Ok(result);
        }

        // POST: api/Patient
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var patient = _mapper.Map<Patient>(dto);

            var created = await _patientRepository.AddAsync(patient);

            var result = _mapper.Map<PatientDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.PatientId }, result);
        }

        // PUT: api/Patient/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.PatientId) return BadRequest("Id mismatch");

            var patient = _mapper.Map<Patient>(dto);

            var updated = await _patientRepository.UpdateAsync(patient);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Patient/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _patientRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}