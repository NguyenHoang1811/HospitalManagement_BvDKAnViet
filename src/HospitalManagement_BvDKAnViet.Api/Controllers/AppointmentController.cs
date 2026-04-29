using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.AppointmentDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.Enums;
using HospitalManagement_BvDKAnViet.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [Authorize]
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
        // Admin/Staff: get all appointments
        // Doctor: get appointments only for the logged-in doctor
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentRepository.GetAllAsync();

            if (User.IsInRole("Doctor"))
            {
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                    return Forbid();

                var doctorAppointments = appointments.Where(a => a.DoctorId == userId);
                var result = _mapper.Map<IEnumerable<AppointmentDto>>(doctorAppointments);
                return Ok(result);
            }

            // Admin and Staff can see all
            if (User.IsInRole("Admin") || User.IsInRole("Staff"))
            {
                var result = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return Ok(result);
            }

            return Forbid();
        }

        // GET: api/Appointment/5
        // Admin/Staff: can view any
        // Doctor: only if appointment belongs to them
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment is null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                    return Forbid();

                if (appointment.DoctorId != userId) return Forbid();
            }
            else if (!(User.IsInRole("Admin") || User.IsInRole("Staff")))
            {
                return Forbid();
            }

            var result = _mapper.Map<AppointmentDto>(appointment);
            return Ok(result);
        }

        // POST: api/Appointment
        // Only Staff can create appointments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // kiem tra xem benh nhan va bac si co ton tai khong
            var patientExists = await _patientRepository.GetByIdAsync(dto.PatientId) != null;
            var doctorExists = await _doctorRepository.GetByIdAsync(dto.DoctorId) != null;

            if (!patientExists || !doctorExists)
                return BadRequest("Patient or Doctor not found");

            // Parse time
            if (!TimeOnly.TryParse(dto.AppointmentTime, out var time))
                return BadRequest("Sai định dạng giờ. Ví dụ: 14:30");

            // Gộp Date + Time
            var appointmentDateTime = dto.AppointmentDate.ToDateTime(time);
            var now = DateTime.Now;

            // Không quá khứ
            if (appointmentDateTime < now)
                return BadRequest("Không được đặt lịch trong quá khứ");

            // Không cuối tuần
            if (dto.AppointmentDate.DayOfWeek == DayOfWeek.Saturday ||
                dto.AppointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return BadRequest("Chỉ đặt lịch từ thứ 2 đến thứ 6");
            }

            // Giờ hành chính (08:00 - 17:00)
            var start = new TimeOnly(8, 0);
            var end = new TimeOnly(17, 0);

            if (time < start || time > end)
            {
                return BadRequest("Chỉ đặt lịch trong giờ hành chính (08:00 - 17:00)");
            }

            // Nghỉ trưa (12:00 - 13:00)
            var lunchStart = new TimeOnly(12, 0);
            var lunchEnd = new TimeOnly(13, 0);

            if (time >= lunchStart && time < lunchEnd)
            {
                return BadRequest("Không thể đặt lịch trong giờ nghỉ trưa (12:00 - 13:00)");
            }

            // Slot 15 phút 
            if (time.Minute % 15 != 0)
            {
                return BadRequest("Chỉ được đặt lịch theo mỗi 15 phút (08:00, 08:15, 80:30...)");
            }

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
        // Staff can update appointments
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _appointmentRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var patientExists = await _patientRepository.GetByIdAsync(dto.PatientId) != null;
            var doctorExists = await _doctorRepository.GetByIdAsync(dto.DoctorId) != null;

            if (!patientExists || !doctorExists)
                return BadRequest("Patient or Doctor not found");

            // Parse time
            if (!TimeOnly.TryParse(dto.AppointmentTime, out var time))
                return BadRequest("Sai định dạng giờ. Ví dụ: 14:30");

            // Gộp Date + Time
            var appointmentDateTime = dto.AppointmentDate.ToDateTime(time);
            var now = DateTime.Now;

            // Không quá khứ
            if (appointmentDateTime < now)
                return BadRequest("Không được đặt lịch trong quá khứ");

            // Không cuối tuần
            if (dto.AppointmentDate.DayOfWeek == DayOfWeek.Saturday ||
                dto.AppointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return BadRequest("Chỉ đặt lịch từ thứ 2 đến thứ 6");
            }

            // Giờ hành chính (08:00 - 17:00)
            var start = new TimeOnly(8, 0);
            var end = new TimeOnly(17, 0);

            if (time < start || time > end)
            {
                return BadRequest("Chỉ đặt lịch trong giờ hành chính (08:00 - 17:00)");
            }

            // Nghỉ trưa (12:00 - 13:00)
            var lunchStart = new TimeOnly(12, 0);
            var lunchEnd = new TimeOnly(13, 0);

            if (time >= lunchStart && time < lunchEnd)
            {
                return BadRequest("Không thể đặt lịch trong giờ nghỉ trưa (12:00 - 13:00)");
            }

            // Slot 15 phút 
            if (time.Minute % 15 != 0)
            {
                return BadRequest("Chỉ được đặt lịch theo mỗi 15 phút (08:00, 08:15, 80:30...)");
            }

            // check availability excluding this appointment
            var doctorAvailable = await _appointmentRepository.IsDoctorAvailableAsync(dto.DoctorId, dto.AppointmentDate, time, excludeAppointmentId: id);
            if (!doctorAvailable)
                return BadRequest("Doctor is not available at the selected date/time.");

            var patientAvailable = await _appointmentRepository.IsPatientAvailableAsync(dto.PatientId, dto.AppointmentDate, time, excludeAppointmentId: id);
            if (!patientAvailable)
                return BadRequest("Patient already has an appointment at the selected date/time.");

            _mapper.Map(dto, existing);

            var updated = await _appointmentRepository.UpdateAsync(existing);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Appointment/5
        // Only Admin can delete appointments
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _appointmentRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }


        [HttpGet("doctor/{doctorId:int}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetByDoctorId(int doctorId)
        {
            // 🔒 Lấy doctorId từ token (KHÔNG tin doctorId từ URL)
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;

            if (!int.TryParse(doctorIdClaim, out var loggedDoctorId))
                return Forbid();

            // ❗ Nếu cố tình gọi doctor khác → chặn
            if (doctorId != loggedDoctorId)
                return Forbid();

            var appointments = await _appointmentRepository.GetAllAsync();

            var doctorAppointments = appointments
                .Where(a => a.DoctorId == doctorId);

            var result = _mapper.Map<IEnumerable<AppointmentDto>>(doctorAppointments);

            return Ok(result);
        }

        [HttpGet("doctor/{doctorId:int}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateOnly date)
        {
            // 🔒 bảo mật nếu là Doctor
            if (User.IsInRole("Doctor"))
            {
                var claim = User.FindFirst("DoctorId")?.Value;
                if (!int.TryParse(claim, out var loggedDoctorId) || loggedDoctorId != doctorId)
                    return Forbid();
            }

            // rule hệ thống
            var start = new TimeOnly(8, 0);
            var end = new TimeOnly(17, 0);
            var lunchStart = new TimeOnly(12, 0);
            var lunchEnd = new TimeOnly(13, 0);

            var allSlots = new List<TimeOnly>();

            for (var t = start; t <= end; t = t.AddMinutes(15))
            {
                if (t >= lunchStart && t < lunchEnd) continue;
                allSlots.Add(t);
            }

            // lấy lịch đã đặt
            var booked = await _appointmentRepository
                .GetByDoctorIdAndDateAsync(doctorId, date);

            var bookedTimes = booked.Select(a => a.AppointmentTime).ToHashSet();

            // slot trống
            var available = allSlots
                .Where(t => !bookedTimes.Contains(t))
                .Select(t => t.ToString("HH:mm"));

            return Ok(available);
        }

        // GET: api/Appointment/patient/{patientId}
        [HttpGet("patient/{patientId:int}")]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            // Nếu là Patient → chỉ được xem lịch của chính mình
            if (User.IsInRole("Patient"))
            {
                var claim = User.FindFirst("PatientId")?.Value;
                if (!int.TryParse(claim, out var loggedPatientId) || loggedPatientId != patientId)
                    return Forbid();
            }

            var appointments = await _appointmentRepository.GetAllAsync();

            var result = appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime);

            return Ok(_mapper.Map<IEnumerable<AppointmentDto>>(result));
        }
    }
}