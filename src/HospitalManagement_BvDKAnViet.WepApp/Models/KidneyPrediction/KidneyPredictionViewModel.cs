// Models/ViewModels/KidneyPredictionViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.ViewModels
{
    public class KidneyPredictionViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân.")]
        public int PatientId { get; set; }
        public string? PatientName { get; set; }

        // ===================== CHỈ SỐ LÂM SÀNG =====================

        [Required(ErrorMessage = "Vui lòng nhập tuổi.")]
        [Range(0, 120, ErrorMessage = "Tuổi phải từ 0 đến 120.")]
        public double Age { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập huyết áp.")]
        [Range(0, 300, ErrorMessage = "Huyết áp phải từ 0 đến 300 mmHg.")]
        public double BloodPressure { get; set; }

        [Range(1.000, 1.050, ErrorMessage = "Tỷ trọng nước tiểu (sg) phải từ 1.000 đến 1.050.")]
        public double SpecificGravity { get; set; }

        [Range(0, 5, ErrorMessage = "Albumin (al) phải từ 0 đến 5.")]
        public double Albumin { get; set; }

        [Range(0, 5, ErrorMessage = "Đường niệu (su) phải từ 0 đến 5.")]
        public double Sugar { get; set; }

        [Range(0, 500, ErrorMessage = "Glucose máu (bgr) phải từ 0 đến 500 mg/dL.")]
        public double BloodGlucoseRandom { get; set; }

        [Range(0, 200, ErrorMessage = "Ure máu (bu) phải từ 0 đến 200 mg/dL.")]
        public double BloodUrea { get; set; }

        [Range(0, 20, ErrorMessage = "Creatinine huyết thanh (sc) phải từ 0 đến 20 mg/dL.")]
        public double SerumCreatinine { get; set; }

        [Range(100, 200, ErrorMessage = "Natri (sod) phải từ 100 đến 200 mEq/L.")]
        public double Sodium { get; set; }

        [Range(2, 8, ErrorMessage = "Kali (pot) phải từ 2 đến 8 mEq/L.")]
        public double Potassium { get; set; }

        [Range(0, 20, ErrorMessage = "Hemoglobin (hemo) phải từ 0 đến 20 g/dL.")]
        public double Haemoglobin { get; set; }

        [Range(0, 70, ErrorMessage = "Thể tích khối hồng cầu (pcv) phải từ 0 đến 70 %.")]
        public double PackedCellVolume { get; set; }

        [Range(0, 20000, ErrorMessage = "Số lượng bạch cầu (wc) phải từ 0 đến 20000 cells/cumm.")]
        public double WhiteBloodCellCount { get; set; }

        [Range(0, 10, ErrorMessage = "Số lượng hồng cầu (rc) phải từ 0 đến 10 millions/cmm.")]
        public double RedBloodCellCount { get; set; }


        // ===================== XÉT NGHIỆM & BỆNH NỀN =====================
        // Lưu ý: Dựa vào form Razor View trước đó bạn dùng string (ví dụ "normal", "yes")
        // Tôi sử dụng RegularExpression để bắt buộc dữ liệu chỉ được nằm trong các giá trị cho phép.

        [RegularExpression("normal|abnormal", ErrorMessage = "Hồng cầu chỉ được chọn Bình thường hoặc Bất thường.")]
        public string? RedBloodCells { get; set; }

        [RegularExpression("normal|abnormal", ErrorMessage = "Bạch cầu chỉ được chọn Bình thường hoặc Bất thường.")]
        public string? PusCell { get; set; }

        [RegularExpression("present|notpresent", ErrorMessage = "Cặn bạch cầu dữ liệu không hợp lệ.")]
        public string? PusCellClumps { get; set; }

        [RegularExpression("present|notpresent", ErrorMessage = "Vi khuẩn dữ liệu không hợp lệ.")]
        public string? Bacteria { get; set; }

        [RegularExpression("yes|no", ErrorMessage = "Tăng huyết áp chỉ chọn Có hoặc Không.")]
        public string? Hypertension { get; set; }

        [RegularExpression("yes|no", ErrorMessage = "Tiểu đường chỉ chọn Có hoặc Không.")]
        public string? DiabetesMellitus { get; set; }

        [RegularExpression("yes|no", ErrorMessage = "Bệnh mạch vành chỉ chọn Có hoặc Không.")]
        public string? CoronaryArteryDisease { get; set; }

        [RegularExpression("good|poor", ErrorMessage = "Sự thèm ăn chỉ chọn Tốt hoặc Kém.")]
        public string? Appetite { get; set; }

        [RegularExpression("yes|no", ErrorMessage = "Phù chân chỉ chọn Có hoặc Không.")]
        public string? PedalEdema { get; set; }

        [RegularExpression("yes|no", ErrorMessage = "Thiếu máu chỉ chọn Có hoặc Không.")]
        public string? Anemia { get; set; }
    }

    public class KidneyPredictionResultViewModel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [JsonPropertyName("predictionResult")]
        public string? PredictionResult { get; set; }

        [JsonPropertyName("probabilityCkd")]
        public double? ProbabilityCkd { get; set; }

        [JsonPropertyName("probabilityNotCkd")]
        public double? ProbabilityNotCkd { get; set; }

        [JsonPropertyName("riskLevel")]
        public string? RiskLevel { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }       // ✅ đúng tên API
    }
}