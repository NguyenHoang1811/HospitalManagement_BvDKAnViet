using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicineDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly IMapper _mapper;

        public MedicineController(IMedicineRepository medicineRepository, IMapper mapper)
        {
            _medicineRepository = medicineRepository;
            _mapper = mapper;
        }

        // GET: api/Medicine
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _medicineRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<MedicineDto>>(items);
            return Ok(result);
        }

        // GET: api/Medicine/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _medicineRepository.GetByIdAsync(id);
            if (item is null) return NotFound();
            var result = _mapper.Map<MedicineDto>(item);
            return Ok(result);
        }

        // POST: api/Medicine
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicineDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<Medicine>(dto);
            var created = await _medicineRepository.AddAsync(entity);
            var result = _mapper.Map<MedicineDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.MedicineId }, result);
        }

        // PUT: api/Medicine/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicineDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _medicineRepository.GetByIdAsync(id);
            if (existing is null) return NotFound();

            _mapper.Map(dto, existing);

            var updated = await _medicineRepository.UpdateAsync(existing);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Medicine/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _medicineRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
