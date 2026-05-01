using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement_BvDKAnViet.Core.DTOs.KidneyPrediction;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;

namespace HospitalManagement_BvDKAnViet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KidneyPredictionController : ControllerBase
    {
        private readonly IKidneyPredictionRepository _repo;
        private readonly ICkdPythonService _ckdService;
        private readonly IPatientRepository _patientRepo;
        private readonly IMapper _mapper;

        public KidneyPredictionController(
            IKidneyPredictionRepository repo,
            ICkdPythonService ckdService,
            IPatientRepository patientRepo,
            IMapper mapper)
        {
            _repo = repo;
            _ckdService = ckdService;
            _patientRepo = patientRepo;
            _mapper = mapper;
        }

        // Map string → int cho Python API
        private static int MapBinary(string? value, string positiveValue)
            => string.Equals(value, positiveValue, StringComparison.OrdinalIgnoreCase) ? 1 : 0;

        [HttpPost("predict")]
        public async Task<IActionResult> Predict([FromBody] KidneyPredictionRequestDto dto)
        {
            var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
            var doctorId = int.Parse(User.Claims.First(c => c.Type == "DoctorId").Value);
            if (patient is null)
                return NotFound(new { responseCode = 404, responseMessage = "Không tìm thấy bệnh nhân" });

            // Build request cho Python API
            var pythonRequest = new CkdPythonRequest
            {
                age = dto.Age ?? 0,
                bp = dto.BloodPressure ?? 0,
                sg = dto.SpecificGravity ?? 0,
                al = dto.Albumin ?? 0,
                su = dto.Sugar ?? 0,
                rbc = MapBinary(dto.RedBloodCells, "abnormal"),
                pc = MapBinary(dto.PusCell, "abnormal"),
                pcc = MapBinary(dto.PusCellClumps, "present"),
                ba = MapBinary(dto.Bacteria, "present"),
                bgr = dto.BloodGlucoseRandom ?? 0,
                bu = dto.BloodUrea ?? 0,
                sc = dto.SerumCreatinine ?? 0,
                sod = dto.Sodium ?? 0,
                pot = dto.Potassium ?? 0,
                hemo = dto.Hemoglobin ?? 0,
                pcv = dto.PackedCellVolume ?? 0,
                wc = dto.WhiteBloodCellCount ?? 0,
                rc = dto.RedBloodCellCount ?? 0,
                htn = MapBinary(dto.Hypertension, "yes"),
                dm = MapBinary(dto.Diabetes, "yes"),
                cad = MapBinary(dto.CoronaryArteryDisease, "yes"),
                appet = MapBinary(dto.Appetite, "good"),
                pe = MapBinary(dto.PedalEdema, "yes"),
                ane = MapBinary(dto.Anemia, "yes"),
            };

            var pythonResult = await _ckdService.PredictAsync(pythonRequest);
            if (pythonResult is null)
                return StatusCode(502, new { responseCode = 502, responseMessage = "Không kết nối được với AI server" });

            // Lưu vào DB
            var entity = new KidneyPrediction
            {
                PatientId = dto.PatientId,
                DoctorId = doctorId, // ✅ QUAN TRỌNG

                Age = dto.Age,
                BloodPressure = dto.BloodPressure,
                SpecificGravity = dto.SpecificGravity,
                Albumin = dto.Albumin,
                Sugar = dto.Sugar,
                RedBloodCells = dto.RedBloodCells,
                PusCell = dto.PusCell,
                PusCellClumps = dto.PusCellClumps,
                Bacteria = dto.Bacteria,
                BloodGlucoseRandom = dto.BloodGlucoseRandom,
                BloodUrea = dto.BloodUrea,
                SerumCreatinine = dto.SerumCreatinine,
                Sodium = dto.Sodium,
                Potassium = dto.Potassium,
                Hemoglobin = dto.Hemoglobin,
                PackedCellVolume = dto.PackedCellVolume,
                WhiteBloodCellCount = dto.WhiteBloodCellCount,
                RedBloodCellCount = dto.RedBloodCellCount,
                Hypertension = dto.Hypertension,
                Diabetes = dto.Diabetes,
                CoronaryArteryDisease = dto.CoronaryArteryDisease,
                Appetite = dto.Appetite,
                PedalEdema = dto.PedalEdema,
                Anemia = dto.Anemia,
                PredictionResult = pythonResult.label,
            };

            var saved = await _repo.AddAsync(entity);

            return Ok(new
            {
                responseCode = 200,
                responseMessage = "Dự đoán thành công",
                data = new KidneyPredictionResponseDto
                {
                    PredictionId = saved.PredictionId,
                    PatientId = saved.PatientId,
                    PatientName = patient.Name,
                    PredictionResult = pythonResult.label,
                    ProbabilityCkd = pythonResult.probability_ckd,
                    ProbabilityNotCkd = pythonResult.probability_not_ckd,
                    RiskLevel = pythonResult.risk_level,
                    CreatedAt = saved.CreatedAt
                }
            });
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<List<KidneyPredictionResponseDto>>> GetByPatient(int patientId)
        {
            var list = await _repo.GetByPatientIdAsync(patientId);

            var result = _mapper.Map<List<KidneyPredictionResponseDto>>(list);

            return Ok(result);
        }

        [HttpGet("doctor")]
        public async Task<ActionResult<List<KidneyPredictionResponseDto>>> GetByDoctor()
        {
            var doctorIdClaim = User.FindFirst("DoctorId");

            if (doctorIdClaim == null)
                return Unauthorized("Không tìm thấy DoctorId trong token");

            var doctorId = int.Parse(doctorIdClaim.Value);

            var list = await _repo.GetByDoctorIdAsync(doctorId);

            var result = _mapper.Map<List<KidneyPredictionResponseDto>>(list);

            return Ok(result);
        }
    }
}