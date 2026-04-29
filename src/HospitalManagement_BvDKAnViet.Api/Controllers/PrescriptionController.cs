using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.PrescriptionDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public PrescriptionController(
            IPrescriptionRepository prescriptionRepository,
            IMedicalRecordRepository medicalRecordRepository,
            ApplicationDbContext db,
            IMapper mapper)
        {
            _prescriptionRepository = prescriptionRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _db = db;
            _mapper = mapper;
        }

        // GET: api/Prescription
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _prescriptionRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<PrescriptionDto>>(items);
            return Ok(result);
        }

        // GET: api/Prescription/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _prescriptionRepository.GetByIdAsync(id);
            if (item is null) return NotFound();
            var result = _mapper.Map<PrescriptionDto>(item);
            return Ok(result);
        }

        // POST: api/Prescription
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // validate related MedicalRecord exists
            var recordExists = await _medicalRecordRepository.GetByIdAsync(dto.RecordId) != null;
            var medicineExists = await _db.Medicines.AnyAsync(m => m.MedicineId == dto.MedicineId);

            if (!recordExists || !medicineExists)
                return BadRequest("MedicalRecord or Medicine not found");

            var entity = _mapper.Map<Prescription>(dto);
            var created = await _prescriptionRepository.AddAsync(entity);
            var result = _mapper.Map<PrescriptionDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.PrescriptionId }, result);
        }

        // PUT: api/Prescription/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePrescriptionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _prescriptionRepository.GetByIdAsync(id);
            if (existing is null) return NotFound();

            // validate related entities
            var recordExists = await _medicalRecordRepository.GetByIdAsync(dto.RecordId) != null;
            var medicineExists = await _db.Medicines.AnyAsync(m => m.MedicineId == dto.MedicineId);

            if (!recordExists || !medicineExists)
                return BadRequest("MedicalRecord or Medicine not found");

            _mapper.Map(dto, existing);

            var updated = await _prescriptionRepository.UpdateAsync(existing);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Prescription/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _prescriptionRepository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // GET: api/Prescription/record/5
        [HttpGet("record/{recordId:int}")]
        public async Task<IActionResult> GetByRecord(int recordId)
        {
            var items = await _prescriptionRepository.GetByRecordIdAsync(recordId);

            var result = _mapper.Map<IEnumerable<PrescriptionDto>>(items);

            return Ok(result);
        }
    }
}