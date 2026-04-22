using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.PrescriptionDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMapper _mapper;

        public PrescriptionController(IPrescriptionRepository prescriptionRepository, IMapper mapper)
        {
            _prescriptionRepository = prescriptionRepository;
            _mapper = mapper;
        }

        // Doctor: create prescription
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto dto)
        {
            // create logic...
            return StatusCode(201);
        }

        // Admin: view prescriptions
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _prescriptionRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PrescriptionDto>>(list));
        }
    }
}
