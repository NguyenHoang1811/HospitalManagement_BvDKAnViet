using HospitalManagement_BvDKAnViet.WepApp.Models.PatientDTO;
using HospitalManagement_BvDKAnViet.WepApp.Models.ViewModels;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagement_BvDKAnViet.WepApp.Controllers
{
    [Authorize]
    public class KidneyPredictionController : Controller
    {
        private readonly IApiService _apiService;

        public KidneyPredictionController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            await LoadPatientsAsync();
            return View(new KidneyPredictionViewModel());
        }

        // ================= PREDICT =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(KidneyPredictionViewModel vm)
        {
            await LoadPatientsAsync();

            if (!ModelState.IsValid)
                return View("Index", vm);

            try
            {
                // Lấy thông tin bệnh nhân
                var patient = await _apiService.GetAsync<PatientDto>($"api/patient/{vm.PatientId}");
                vm.PatientName = patient?.Name;

                var requestDto = new
                {
                    patientId = vm.PatientId,
                    age = vm.Age,
                    bloodPressure = vm.BloodPressure,
                    specificGravity = vm.SpecificGravity,
                    albumin = vm.Albumin,
                    sugar = vm.Sugar,
                    redBloodCells = vm.RedBloodCells,
                    pusCell = vm.PusCell,
                    pusCellClumps = vm.PusCellClumps,
                    bacteria = vm.Bacteria,
                    bloodGlucoseRandom = vm.BloodGlucoseRandom,
                    bloodUrea = vm.BloodUrea,
                    serumCreatinine = vm.SerumCreatinine,
                    sodium = vm.Sodium,
                    potassium = vm.Potassium,
                    haemoglobin = vm.Haemoglobin,
                    packedCellVolume = vm.PackedCellVolume,
                    whiteBloodCellCount = vm.WhiteBloodCellCount,
                    redBloodCellCount = vm.RedBloodCellCount,
                    hypertension = vm.Hypertension,
                    diabetesMellitus = vm.DiabetesMellitus,
                    coronaryArteryDisease = vm.CoronaryArteryDisease,
                    appetite = vm.Appetite,
                    pedalEdema = vm.PedalEdema,
                    anemia = vm.Anemia
                };

                var result = await _apiService.PredictKidneyAsync(requestDto);

                if (result == null)
                {
                    TempData["Error"] = "Không thể kết nối AI server.";
                    return View("Index", vm);
                }

                result.PatientId = vm.PatientId;
                result.PatientName = vm.PatientName;

                ViewBag.Result = result;

                return View("Index", vm);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return View("Index", vm);
            }
        }

        // ================= HISTORY =================
        public async Task<IActionResult> History(int patientId)
        {
            try
            {
                var data = await _apiService.GetKidneyHistoryAsync(patientId);
                ViewBag.PatientId = patientId;

                return View(data ?? new List<KidneyPredictionResultViewModel>());
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Không thể tải lịch sử: {ex.Message}";
                return View(new List<KidneyPredictionResultViewModel>());
            }
        }

        
        // ================= DOCTOR LIST =================
        public async Task<IActionResult> MyPredictions(string searchTerm, int page = 1, int pageSize = 10)
        {
            try
            {
                // 1. Lấy toàn bộ lịch sử dự đoán của bác sĩ
                var data = await _apiService.GetMyPredictionsAsync()
                           ?? new List<KidneyPredictionResultViewModel>();

                // 2. Logic Tìm kiếm (Theo tên bệnh nhân hoặc ID)
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    data = data.Where(d =>
                        (!string.IsNullOrEmpty(d.PatientName) && d.PatientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (d.PatientId.ToString() == searchTerm)
                    ).ToList();
                }

                // 3. Logic Phân trang
                int totalItems = data.Count();
                var pagedData = data
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 4. Truyền trạng thái ra View
                ViewBag.SearchTerm = searchTerm;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = totalItems == 0 ? 1 : (int)Math.Ceiling((double)totalItems / pageSize);

                return View(pagedData);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Không thể tải dữ liệu: {ex.Message}";
                return View(new List<KidneyPredictionResultViewModel>());
            }
        }

        // ================= PRIVATE =================
        private async Task LoadPatientsAsync()
        {
            if (User.IsInRole("Doctor"))
            {
                var doctorId = User.FindFirstValue("DoctorId") ?? "0";
                ViewBag.Patients = await _apiService.GetAsync<List<PatientDto>>($"api/patient/doctor/{doctorId}");
            }
            else
            {
                ViewBag.Patients = await _apiService.GetAsync<List<PatientDto>>("api/patient");
            }
        }
    }
}