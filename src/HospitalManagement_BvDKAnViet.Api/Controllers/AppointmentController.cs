using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.AppointmentDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.Enums;
using HospitalManagement_BvDKAnViet.Core.IServies;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public AppointmentController(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        // GET: api/Appointment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(result);
        }

        // GET: api/Appointment/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment is null) return NotFound();

            var result = _mapper.Map<AppointmentDto>(appointment);
            return Ok(result);
        }

        // POST: api/Appointment
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // kiem tra xem benh nhan va bac si co ton tai khong
            var patientExists = await _patientRepository.GetByIdAsync(dto.PatientId) != null;
            var doctorExists = await _doctorRepository.GetByIdAsync(dto.DoctorId) != null;

            if (!patientExists || !doctorExists)
                return BadRequest("Patient or Doctor not found");

            if (!TimeOnly.TryParse(dto.AppointmentTime, out var time))
                return BadRequest("Invalid AppointmentTime format. Expected e.g. '14:30'.");

            // kiem tra xem bac si va benh nhan co trung lich khong
            var doctorAvailable = await _appointmentRepository.IsDoctorAvailableAsync(dto.DoctorId, dto.AppointmentDate, time);
            if (!doctorAvailable)
                return BadRequest("Doctor is not available at the selected date/time.");

            var patientAvailable = await _appointmentRepository.IsPatientAvailableAsync(dto.PatientId, dto.AppointmentDate, time);
            if (!patientAvailable)
                return BadRequest("Patient already has an appointment at the selected date/time.");

            var appointment = _mapper.Map<Appointment>(dto);

            // mac dinh moi tao se co trang thai Pending
            appointment.Status = (int)AppointmentStatus.PENDING;
            appointment.StatusName = AppointmentStatus.PENDING.ToString();

            var created = await _appointmentRepository.AddAsync(appointment);
            var result = _mapper.Map<AppointmentDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, result);
        }

        // PUT: api/Appointment/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _appointmentRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var patientExists = await _patientRepository.GetByIdAsync(dto.PatientId) != null;
            var doctorExists = await _doctorRepository.GetByIdAsync(dto.DoctorId) != null;

            if (!patientExists || !doctorExists)
                return BadRequest("Patient or Doctor not found");

            if (!TimeOnly.TryParse(dto.AppointmentTime, out var time))
                return BadRequest("Invalid AppointmentTime format. Expected e.g. '14:30'.");

            // kiem tra xem bac si va benh nhan co trung lich khong, excludeAppointmentId de cho phep update ma khong bi loi trung lich voi chinh no
            var doctorAvailable = await _appointmentRepository.IsDoctorAvailableAsync(dto.DoctorId, dto.AppointmentDate, time, excludeAppointmentId: id);
            if (!doctorAvailable)
                return BadRequest("Doctor is not available at the selected date/time.");

            var patientAvailable = await _appointmentRepository.IsPatientAvailableAsync(dto.PatientId, dto.AppointmentDate, time, excludeAppointmentId: id);
            if (!patientAvailable)
                return BadRequest("Patient already has an appointment at the selected date/time.");

            _mapper.Map(dto, existing);

            // If caller didn't set status, keep existing; otherwise mapping may overwrite.
            // Ensure StatusName consistency if Status is numeric; keep as-is here.

            var updated = await _appointmentRepository.UpdateAsync(existing);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Appointment/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _appointmentRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}