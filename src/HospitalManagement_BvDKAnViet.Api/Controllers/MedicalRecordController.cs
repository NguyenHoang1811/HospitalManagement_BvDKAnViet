using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using HospitalManagement_BvDKAnViet.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // authenticated required; role checks per action
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

        public MedicalRecordController(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

        // POST: api/MedicalRecord
        // Doctor can create medical records
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordDto dto)
        {
            // validation + mapping + persist...
            return StatusCode(201);
        }

        // GET: api/MedicalRecord
        // Admin: view all records
        // Doctor: view only records created by them (or their patients)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (User.IsInRole("Admin"))
            {
                var all = await _medicalRecordRepository.GetAllAsync();
                return Ok(_mapper.Map<IEnumerable<MedicalRecordDto>>(all));
            }

            if (User.IsInRole("Doctor"))
            {
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                    return Forbid();

                var list = await _medicalRecordRepository.GetByIdAsync(userId);
                return Ok(_mapper.Map<IEnumerable<MedicalRecordDto>>(list));
            }

            return Forbid();
        }

        // GET: api/MedicalRecord/5
        // Admin can view any; Doctor only if the record belongs to them
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(id);
            if (record == null) return NotFound();

            if (User.IsInRole("Admin")) return Ok(_mapper.Map<MedicalRecordDto>(record));

            if (User.IsInRole("Doctor"))
            {
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                    return Forbid();

                if (record.DoctorId != userId) return Forbid();

                return Ok(_mapper.Map<MedicalRecordDto>(record));
            }

            return Forbid();
        }
    }
}