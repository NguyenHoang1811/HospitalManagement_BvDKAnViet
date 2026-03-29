using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public MedicalRecordController(
            IMedicalRecordRepository medicalRecordRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        // GET: api/MedicalRecord
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await _medicalRecordRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
            return Ok(result);
        }

        // GET: api/MedicalRecord/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(id);
            if (record is null) return NotFound();

            var result = _mapper.Map<MedicalRecordDto>(record);
            return Ok(result);
        }

        // POST: api/MedicalRecord
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var patientExists = await _patientRepository.GetByIdAsync(dto.PatientId) != null;
            var doctorExists = await _doctorRepository.GetByIdAsync(dto.DoctorId) != null;

            if (!patientExists || !doctorExists)
                return BadRequest("Patient or Doctor not found");

            var record = _mapper.Map<MedicalRecord>(dto);
            var created = await _medicalRecordRepository.AddAsync(record);
            var result = _mapper.Map<MedicalRecordDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.RecordId }, result);
        }

        // PUT: api/MedicalRecord/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicalRecordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.RecordId) return BadRequest("Id mismatch");

            var patientExists = await _patientRepository.GetByIdAsync(dto.PatientId) != null;
            var doctorExists = await _doctorRepository.GetByIdAsync(dto.DoctorId) != null; // will fix below

            // fix: use correct field name
            if (!patientExists || !doctorExists)
                return BadRequest("Patient or Doctor not found");

            var record = _mapper.Map<MedicalRecord>(dto);
            var updated = await _medicalRecordRepository.UpdateAsync(record);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/MedicalRecord/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _medicalRecordRepository.DeleteAsync(id); // will fix below
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}