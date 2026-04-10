using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.DepartmentDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        public DepartmentController(
           IDepartmentRepository departmentRepository,
           IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // GET: api/Department
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _departmentRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
            return Ok(result);
        }

        // GET: api/Department/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department is null) return NotFound();

            var result = _mapper.Map<DepartmentDto>(department);
            return Ok(result);
        }

        // POST: api/Department
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var department = _mapper.Map<Department>(dto);
            var created = await _departmentRepository.AddAsync(department);
            var result = _mapper.Map<DepartmentDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.DepartmentId }, result);
        }

        // PUT: api/Department/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _departmentRepository.GetByIdAsync(id);
            if (existing is null) return NotFound();

            _mapper.Map(dto, existing);

            var updated = await _departmentRepository.UpdateAsync(existing);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Department/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _departmentRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}